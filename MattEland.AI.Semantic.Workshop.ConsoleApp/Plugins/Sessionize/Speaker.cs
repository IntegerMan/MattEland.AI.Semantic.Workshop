namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;

public class Speaker
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string FullName { get; set; }
    public required string Bio { get; set; }
    public required string TagLine { get; set; }
    public required string ProfilePicture { get; set; }
    public List<SpeakerSession> Sessions { get; set; } = new();
}