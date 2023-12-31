﻿using Newtonsoft.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;

public class Category
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("categoryItems")]
    public List<CategoryItem> CategoryItems { get; set; } = new();

    [JsonProperty("sort")]
    public int Sort { get; set; }
}
