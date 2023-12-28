using Newtonsoft.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;

public class SessionSpeaker
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }
}