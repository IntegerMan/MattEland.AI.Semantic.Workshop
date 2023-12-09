using System.ComponentModel;

public enum WorkshopMenuOption
{
    [Description("Part 1: Azure AI Services")]
    Part1,
    [Description("Part 2: Working with OpenAI and LLMs")]
    Part2,
    [Description("Part 3: Semantic Kernel and Semantic Functions")]
    Part3,
    [Description("Part 4: Semantic Kernel Plugins and Planners")]
    Part4,
    [Description("Quit")]
    Quit
}