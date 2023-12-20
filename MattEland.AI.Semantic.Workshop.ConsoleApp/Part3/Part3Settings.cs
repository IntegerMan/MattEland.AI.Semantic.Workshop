﻿namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public class Part3Settings
{
    public Part3Settings(string openAiKey, string? openAiEndpoint, string? textDeployment, string? chatDeployment, string? embeddingDeployment, string? imageDeployment)
    {
        OpenAiKey = openAiKey;
        OpenAiEndpoint = openAiEndpoint;
        TextDeployment = textDeployment;
        ChatDeployment = chatDeployment;
        EmbeddingDeployment = embeddingDeployment;
        ImageDeployment = imageDeployment;
    }

    public string OpenAiKey { get; }
    public string? OpenAiEndpoint { get; }
    public string? TextDeployment { get; }
    public string? ChatDeployment { get; }
    public string? EmbeddingDeployment { get; }
    public string? ImageDeployment { get; }
    public bool ShowTokenUsage { get; }
    public bool ShowFilterResults { get; }
}