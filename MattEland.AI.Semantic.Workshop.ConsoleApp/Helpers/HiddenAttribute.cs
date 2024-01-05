namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;

public class HiddenAttribute : Attribute
{
    public required string Reason { get; set; }
}