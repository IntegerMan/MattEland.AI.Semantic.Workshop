﻿using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;

public class SessionizePlugin
{
    private readonly SessionizeService _sessionize;
    private readonly List<Session> _sessions = new();
    private readonly List<Speaker> _speakers = new();

    public SessionizePlugin(string apiToken)
    {
        _sessionize = new SessionizeService(apiToken);
    }

    private async Task<List<Session>> GetSessionsAsync()
    {
        if (_sessions.Count <= 0)
        {
            IEnumerable<Session> sessions = await _sessionize.GetSessionsAsync();
            _sessions.AddRange(sessions);
        }

        return _sessions;
    }

    private async Task<List<Speaker>> GetSpeakersAsync()
    {
        if (_speakers.Count <= 0)
        {
            IEnumerable<Speaker> speakers = await _sessionize.GetSpeakerEntriesAsync();
            _speakers.AddRange(speakers);
        }

        return _speakers;
    }

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
        if (!DateTime.TryParse(dateTime, out DateTime activeDateTime))
        {
            return $"{dateTime} is not a valid date / time";
        }

        IEnumerable<Session> sessions = (await GetSessionsAsync()).Where(s => s.StartsAt <= activeDateTime && s.EndsAt >= activeDateTime).ToList();

        if (!sessions.Any())
            return $"There are no active sessions at {dateTime}";

        return "Active sessions during this time are " + string.Join(", ",
            sessions.OrderBy(s => s.StartsAt).ThenBy(s => s.Title).Select(s => s.Title));
    }

    [KernelFunction, Description("Gets the unique dates of the conference")]
    public async Task<string> GetAllSessionDates()
    {
        IEnumerable<Session> sessions = await GetSessionsAsync();
        IEnumerable<DateTime> dates = sessions.Select(s => s.StartsAt.Date).Distinct().OrderBy(d => d);

        return string.Join(", ", dates.Select(d => d.ToShortDateString()));
    }

    [KernelFunction, Description("Gets the unique session start times")]
    public async Task<string> GetAllSessionStartTimes()
    {
        IEnumerable<Session> sessions = await GetSessionsAsync();
        IEnumerable<DateTime> dates = sessions.Select(s => s.StartsAt).Distinct().OrderBy(d => d);

        return string.Join(", ", dates.Select(d => d.ToString("f")));
    }

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

    [KernelFunction, Description("Gets the title of all sessions in a specific room")]
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
    public async Task<string> GetSessionDetails([Description("The title of the session")] string title)
    {
        IEnumerable<Session> sessions = await GetSessionsAsync();
        Session? session = sessions.FirstOrDefault(s => string.Equals(s.Title, title, StringComparison.OrdinalIgnoreCase));

        if (session == null)
        {
            return $"Could not find a session named '{title}'";
        }

        return BuildSessionString(session);
    }

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

    [KernelFunction, Description("Gets speaker details by their full name")]
    public async Task<string> GetSpeakerDetails([Description("The full name of the speaker")] string fullName)
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

    public void Dispose()
    {
        _sessionize.Dispose();
    }
}