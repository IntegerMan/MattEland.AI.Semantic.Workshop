namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;
public class SessionGroup
{
    public required string GroupId { get; set; } // TODO: Probably a GUID, but I don't have a value in reference data so string is safer
    public required string GroupName { get; set; }
    public List<Session> Sessions { get; set; } = new();
}
