namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;

public class Part1Settings
{
    public Part1Settings(string aiKey, string aiEndpoint, string aiRegion)
    {
        AiKey = aiKey;
        AiEndpoint = aiEndpoint;
        AiRegion = aiRegion;
    }

    public string AiKey { get; }
    public string AiEndpoint { get; }
    public string AiRegion { get; }
    public string VoiceName { get; set; } = "en-GB-AlfieNeural";
}