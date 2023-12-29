namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;
public class SessionGroup
{
    public required string GroupId { get; set; }
    public required string GroupName { get; set; }
    public List<Session> Sessions { get; set; } = new();
}
