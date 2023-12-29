using Azure;
using Azure.AI.OpenAI;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class EmbeddingDemo
{
    private readonly OpenAIClient _client;
    private readonly AppSettings _settings;

    public EmbeddingDemo(AppSettings settings)
    {
        bool useAzureOpenAI = settings.AzureOpenAI.IsConfigured;

        if (!useAzureOpenAI)
        {
            _client = new OpenAIClient(settings.OpenAI.Key);
        } 
        else
        {
            Uri uri = new(settings.AzureOpenAI.Endpoint);
            AzureKeyCredential key = new(settings.AzureOpenAI.Key);
            _client = new OpenAIClient(uri, key);
        }

        _settings = settings;
    }

    public async Task<float[]> GetEmbeddingsAsync(string prompt)
    {
        bool useAzureOpenAI = _settings.AzureOpenAI.IsConfigured;
        string deployment = useAzureOpenAI 
            ? _settings.AzureOpenAI.EmbeddingDeploymentName 
            : _settings.OpenAI.EmbeddingModel;

        EmbeddingsOptions options = new()
        {
            DeploymentName = deployment
        };

        try
        {
            options.Input.Add(prompt);

            Response<Embeddings> result = await _client.GetEmbeddingsAsync(options);

            ReadOnlyMemory<float> embedding = result.Value.Data.First().Embedding;

            return embedding.ToArray();
        }
        catch (RequestFailedException ex)
        {
            if (ex.ErrorCode == "DeploymentNotFound")
            {
                AnsiConsole.MarkupLine($"[Red]Deployment {options.DeploymentName} not found. Please check your settings.[/]");
            }
            else if (ex.ErrorCode == "MaxTokensError")
            {
                AnsiConsole.MarkupLine($"[Red]Max tokens exceeded. Please try a shorter prompt.[/]");
            }
            else if (ex.ErrorCode == "CompletionNotFoundError")
            {
                AnsiConsole.MarkupLine($"[Red]Completion not found. Please try a different prompt.[/]");
            }
            else if (ex.ErrorCode == "OperationNotSupported")
            {
                AnsiConsole.MarkupLine($"[Red]Operation not supported. You may need to deploy a different model. text-embedding-ada-002 is known to work.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[Red]Error: {ex.Message} ({ex.GetType().Name})[/]");
                AnsiConsole.MarkupLine($"[Red]Status Code: {ex.Status}[/]");
                AnsiConsole.MarkupLine($"[Red]Error Code: {ex.ErrorCode}[/]");
            }

            return [];
        }
    }
}
