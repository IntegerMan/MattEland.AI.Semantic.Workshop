using Newtonsoft.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;

public class SessionizeService : IDisposable
{
    private readonly HttpClient? _client;
    private readonly string? _apiToken;
    private List<Speaker>? _speakers;
    private List<Session>? _sessions;

    public SessionizeService(string apiToken)
    {
        _apiToken = apiToken;
        _client = new HttpClient(); // This would be better if you could pass it a HttpClientFactory
    }

    public SessionizeService(string sessionsJson, string speakersJson)
    {
        _speakers = JsonConvert.DeserializeObject<List<Speaker>>(speakersJson)!;
        _sessions = JsonConvert.DeserializeObject<List<Session>>(sessionsJson)!;
    }

    public async Task<IEnumerable<Speaker>> GetSpeakerEntriesAsync()
    {
        if (_speakers is not null)
        {
            return _speakers;
        }

        string json = await _client!.GetStringAsync($"https://sessionize.com/api/v2/{_apiToken}/view/Speakers");

        _speakers = JsonConvert.DeserializeObject<List<Speaker>>(json)!;
        return _speakers;
    }

    public async Task<IEnumerable<Session>> GetSessionsAsync()
    {
        if (_sessions is not null)
        {
            return _sessions;
        }

        string json = await _client!.GetStringAsync($"https://sessionize.com/api/v2/{_apiToken}/view/Sessions");

        _sessions = JsonConvert.DeserializeObject<List<SessionGroup>>(json)!.SelectMany(g => g.Sessions).ToList();
        return _sessions;
    }

    public void Dispose() => _client?.Dispose();
}