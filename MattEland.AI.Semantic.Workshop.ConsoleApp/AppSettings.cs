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

    public bool IsConfigured => !string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(Endpoint) && !string.IsNullOrEmpty(Region);
}

public class OpenAISettings
{
    public required string Key { get; init; }
    public required string TextModel { get; init; } = "gpt-4";
    public required string ChatModel { get; init; } = "gpt-4";
    public required string EmbeddingModel { get; init; } = "text-embedding-ada-002";
    public required string ImageModel { get; init; } = "dall-e-3";

    public bool IsConfigured => !string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(TextModel) && !string.IsNullOrEmpty(ChatModel) && !string.IsNullOrEmpty(EmbeddingModel) && !string.IsNullOrEmpty(ImageModel);
}

public class AzureOpenAISettings
{
    public required string Key { get; init; }
    public required string Endpoint { get; init; }
    public required string TextDeploymentName { get; init; }
    public required string ChatDeploymentName { get; init; }
    public required string EmbeddingDeploymentName { get; init; }
    // Image deployments doesn't seem to be available at the moment, sadly.

    public bool IsConfigured => !string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(Endpoint) && !string.IsNullOrEmpty(TextDeploymentName) && !string.IsNullOrEmpty(ChatDeploymentName) && !string.IsNullOrEmpty(EmbeddingDeploymentName);
}