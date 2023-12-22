using Newtonsoft.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;

public class CategoryItem
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }
}