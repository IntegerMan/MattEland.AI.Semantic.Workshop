using MattEland.AI.Semantic.Workshop.ConsoleApp.Properties;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;

public class SessionizePlugin
{
    private readonly SessionizeService _sessionize;

    public SessionizePlugin(string? apiToken = null)
    {
        // This is built to work either with API calls or by using hard-coded JSON. The JSON is particularly helpful for conference WiFi and to prevent DDOSing Sessionize
        if (!string.IsNullOrEmpty(apiToken))
        {
            _sessionize = new SessionizeService(apiToken);
        } 
        else
        {
            _sessionize = new SessionizeService(Resources.SessionizeSessions, Resources.SessionizeSpeakers);
        }
    }

    private async Task<IEnumerable<Session>> GetSessionsAsync() 
        => await _sessionize.GetSessionsAsync();

    private async Task<IEnumerable<Speaker>> GetSpeakersAsync() 
        => await _sessionize.GetSpeakerEntriesAsync();

    [KernelFunction, Description("Gets the names of all speakers for the conference")]
    public async Task<string> GetAllSpeakerNames()
    {
        IEnumerable<Speaker> speakers = await GetSpeakersAsync();

        return string.Join(", ", speakers.OrderBy(s => s.FullName).Select(s => s.FullName).Distinct());
    }

    [KernelFunction, Description("Gets the title of all sessions for the conference")]
    public async Task<string> GetAllSessions()
    {
        IEnumerable<Session> sessions = await GetSessionsAsync();

        return string.Join(", ", sessions.OrderBy(s => s.StartsAt).ThenBy(s => s.Title).Select(s => s.Title));
    }

    [KernelFunction, Description("Gets the titles of all sessions active at a specified time")]
    public async Task<string> GetAllActiveSessionNames([Description("The date and time of the session")] string dateTime)
    {
        if (!TryParseDate(dateTime, out DateTime activeDateTime))
        {
            return $"{dateTime} is not a valid date / time";
        }

        IEnumerable<Session> sessions = (await GetSessionsAsync()).Where(s => s.StartsAt <= activeDateTime && s.EndsAt >= activeDateTime).ToList();

        if (!sessions.Any())
            return $"There are no active sessions at {dateTime}";

        return "Active sessions during this time are " + string.Join(", ",
            sessions.OrderBy(s => s.StartsAt).ThenBy(s => s.Title).Select(s => s.Title));
    }

    /*
    [KernelFunction, Description("Gets the unique dates of the conference")]
    public async Task<string> GetAllSessionDates()
    {
        IEnumerable<Session> sessions = await GetSessionsAsync();
        IEnumerable<DateTime> dates = sessions.Select(s => s.StartsAt.Date).Distinct().OrderBy(d => d);

        return string.Join(", ", dates.Select(d => d.ToShortDateString()));
    }
    */

    /* Disabling to reduce demo complexity
    [KernelFunction, Description("Gets the unique session start times")]
    public async Task<string> GetAllSessionStartTimes()
    {
        IEnumerable<Session> sessions = await GetSessionsAsync();
        IEnumerable<DateTime> dates = sessions.Select(s => s.StartsAt).Distinct().OrderBy(d => d);

        return string.Join(", ", dates.Select(d => d.ToString("f")));
    }
    */

    /*
    [KernelFunction, Description("Gets the unique session start times for a specific day of the conference")]
    public async Task<string> GetAllSessionStartTimesForSpecificDate(
        [Description("The date to retrieve session start times for. For example, 1/9/24")] string date)
    {
        if (!DateTime.TryParse(date, out DateTime dateTime))
        {
            return $"{date} is not a valid date";
        }

        IEnumerable<Session> sessions = await GetSessionsAsync();
        IEnumerable<DateTime> dates = sessions.Select(s => s.StartsAt).Distinct().OrderBy(d => d);

        return string.Join(", ", dates.Select(d => d.ToString("f")));
    }
    */


    [KernelFunction, Description("Gets the session names for a specific day of the conference")]
    public async Task<string> GetAllSessionNamesForSpecificDate(
        [Description("The date to retrieve sessions for. For example, 1/9/24")] string date)
    {
        if (!TryParseDate(date, out DateTime dateTime))
        {
            return $"{date} is not a valid date";
        }

        IEnumerable<Session> sessions = await GetSessionsAsync();
        IEnumerable<Session> matchingSessions = sessions.Where(s => s.StartsAt.Date == dateTime.Date);

        if (!matchingSessions.Any())
        {
            if (sessions.Any(s => s.StartsAt.Date > dateTime.Date))
            {
                return $"There are no sessions on {dateTime.ToShortDateString()}. The next session is on {sessions.OrderBy(s => s.StartsAt).First().StartsAt.ToLongDateString()}";
            }

            return $"There are no sessions on {dateTime.ToShortDateString()}. The last session occurred on {sessions.OrderBy(s => s.StartsAt).Last().StartsAt.ToLongDateString()}";
        }

        return $"Sessions on {dateTime.ToShortDateString()} include " + string.Join(", ", matchingSessions.Select(s => s.Title));
    }

    private static bool TryParseDate(string time, out DateTime dateTime)
    {
        if (time.Equals("today", StringComparison.OrdinalIgnoreCase))
        {
            dateTime = DateTime.Today;
            return true;
        }

        if (time.Equals("tomorrow", StringComparison.OrdinalIgnoreCase))
        {
            dateTime = DateTime.Today.AddDays(1);
            return true;
        }

        if (time.Equals("yesterday", StringComparison.OrdinalIgnoreCase))
        {
            dateTime = DateTime.Today.AddDays(-1);
            return true;
        }

        if (DateTime.TryParse(time, out dateTime))
        {
            return true;
        }
        return false;
    }

    /*
    [KernelFunction, Description("Gets the titles of all upcoming sessions")]
    public async Task<string> GetUpcomingSessionTitles()
    {
        IEnumerable<Session> sessions = (await GetSessionsAsync()).Where(s => s.StartsAt >= DateTime.Now);
        
        if (!sessions.Any())
            return "There are no upcoming sessions";

        return $"Upcoming sessions are {string.Join(", ", sessions.OrderBy(s => s.StartsAt).ThenBy(s => s.Title).Select(s => s.Title))}";
    }

    [KernelFunction, Description("Gets the title of all completed sessions")]
    public async Task<string> GetCompletedSessionTitles()
    {
        IEnumerable<Session> sessions = (await GetSessionsAsync()).Where(s => s.EndsAt <= DateTime.Now);

        if (!sessions.Any())
            return "There are no completed sessions";

        return $"Completed sessions are {string.Join(", ", sessions.OrderBy(s => s.StartsAt).ThenBy(s => s.Title).Select(s => s.Title))}";
    }
    */

    [KernelFunction, Description("Gets the name of all sessions in a specific room")]
    public async Task<string> GetSessionNamesByRoom([Description("The name of the room. For example, River A")] string room)
    {
        IEnumerable<Session> sessions = (await GetSessionsAsync())
            .Where(s => s.Room.Equals(room, StringComparison.OrdinalIgnoreCase));

        if (!sessions.Any())
            return $"Could not find any sessions in room '{room}'";

        return $"Sessions in {room}: {string.Join(", ", sessions.OrderBy(s => s.StartsAt).ThenBy(s => s.Title).Select(s => s.Title))}";
    }

    [KernelFunction, Description("Gets the names of all rooms in the conference")]
    public async Task<string> GetUniqueRooms()
    {
        IEnumerable<Session> sessions = await GetSessionsAsync();
        IEnumerable<string> rooms = sessions.Select(s => s.Room).Distinct();

        if (!rooms.Any())
            return "Could not find any rooms";

        return $"Available rooms are: {string.Join(", ", rooms)}";
    }

    [KernelFunction, Description("Gets the session titles of sessions by a specific speaker")]
    public async Task<string> GetSessionsBySpeaker([Description("The full name of the speaker")] string fullName)
    {
        IEnumerable<Session> sessions = (await GetSessionsAsync()).Where(s => s.Speakers.Exists(sp => sp.Name.Equals(fullName, StringComparison.OrdinalIgnoreCase)));

        if (!sessions.Any())
            return $"There are no sessions by {fullName}";

        return $"{fullName}'s sessions are {string.Join(", ", sessions.OrderBy(s => s.StartsAt).ThenBy(s => s.Title).Select(s => s.Title))}";
    }

    [KernelFunction, Description("Gets session details by a session title")]
    public async Task<string> GetSessionDetails([Description("The name of the session")] string title)
    {
        IEnumerable<Session> sessions = await GetSessionsAsync();
        Session? session = sessions.FirstOrDefault(s => string.Equals(s.Title, title, StringComparison.OrdinalIgnoreCase));

        if (session == null)
        {
            return $"Could not find a session named '{title}'";
        }

        return BuildSessionString(session);
    }

    [KernelFunction, Description("Gets speaker details by their full name")]
    public async Task<string> GetSpeakerDetails([Description("The name of the speaker")] string fullName)
    {
        IEnumerable<Speaker> speakers = await GetSpeakersAsync();
        Speaker? speaker = speakers.FirstOrDefault(s => string.Equals(s.FullName, fullName, StringComparison.OrdinalIgnoreCase));

        if (speaker == null)
        {
            return $"Could not find a speaker named '{fullName}'";
        }

        return BuildSpeakerString(speaker);
    }

    private static string BuildSpeakerString(Speaker speaker)
        => $"{speaker.FullName} is speaking on the following sessions: {string.Join(", ", speaker.Sessions.Select(s => s.Name))}. Their bio follows: \r\n{speaker.Bio}";

    private static string BuildSessionString(Session session)
    {
        StringBuilder sb = new();
        bool isPast = session.EndsAt < DateTime.Now;

        if (session.Speakers.Count == 1)
        {
            sb.Append($"{session.Speakers.First().Name} {(isPast ? "spoke on " : "is speaking on ")}");
        }
        else if (session.Speakers.Count > 1)
        {
            sb.Append($"{string.Join(", ", session.Speakers.Select(s => s.Name))} {(isPast ? "spoke on " : "are speaking on ")}");
        }

        sb.Append($"'{session.Title}' in room {session.Room}");
        sb.Append($" from {session.StartsAt.ToLocalTime().ToShortTimeString()} to {session.EndsAt.ToLocalTime().ToShortTimeString()}");
        sb.Append($" on {session.StartsAt.Date.ToShortDateString()}.");

        List<string> tags = session.Categories.SelectMany(c => c.CategoryItems).Select(t => t.Name).ToList();
        if (tags.Count > 0)
        {
            sb.Append($" The session is tagged with the following categories: {string.Join(", ", tags)}.");
        }
        sb.AppendLine($" The session's abstract follows:");
        sb.AppendLine(session.Description);

        return sb.ToString();
    }

    public void Dispose()
    {
        _sessionize.Dispose();
    }
}
