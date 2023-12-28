namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public class AppSettings
{
    public bool SkipCostDisclaimer { get; init; }
    public required AzureAISettings AzureAIServices { get; init; }
    public required OpenAISettings OpenAI { get; init; }
    public required AzureOpenAISettings AzureOpenAI { get; init; }
    public string? SessionizeApiToken { get; init; }
}

public class AzureAISettings
{
    public required string Key { get; init;}
    public required string Endpoint { get; init; }
    public required string Region { get; init; }
    public string VoiceName { get; init; } = "en-GB-AlfieNeural";
}

public class OpenAISettings
{
    public required string Key { get; init; }
    public required string TextModel { get; init; }
    public required string ChatModel { get; init; }
    public required string EmbeddingModel { get; init; }
    public required string ImageModel { get; init; }
}

public class AzureOpenAISettings
{
    public required string Key { get; init; }
    public required string Endpoint { get; init; }
    public required string TextDeploymentName { get; init; }
    public required string ChatDeploymentName { get; init; }
    public required string EmbeddingDeploymentName { get; init; }
    // Image deployments doesn't seem to be available at the moment, sadly.
}