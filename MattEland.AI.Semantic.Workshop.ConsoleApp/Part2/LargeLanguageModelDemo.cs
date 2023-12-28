using Azure;
using Azure.AI.OpenAI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class LargeLanguageModelDemo
{
    private readonly OpenAIClient _client;
    private readonly AppSettings _settings;

    public LargeLanguageModelDemo(AppSettings settings)
    {
        bool useAzureOpenAI = !string.IsNullOrEmpty(settings.AzureOpenAI.Endpoint);

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

    public async Task<string?> GetTextCompletionAsync(string prompt)
    {
        bool useAzureOpenAI = !string.IsNullOrEmpty(_settings.AzureOpenAI.Endpoint);
        string deployment = useAzureOpenAI 
            ? _settings.AzureOpenAI.TextDeploymentName 
            : _settings.OpenAI.TextModel; // TODO: This may not work. Verify

        CompletionsOptions options = new()
        {
            ChoicesPerPrompt = 1,
            MaxTokens = 1000,
            DeploymentName = deployment,
            Temperature = 1f, // ranges from 0 to 2
            Echo = false,
        };
        options.Prompts.Add(prompt);

        try
        {
            Response<Completions> result = await _client.GetCompletionsAsync(options);

            return result.Value.Choices.First().Text;
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
                AnsiConsole.MarkupLine($"[Red]Operation not supported. You may need to deploy a different model. gpt-35-turbo-instruct is known to work.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[Red]Error: {ex.Message} ({ex.GetType().Name})[/]");
                AnsiConsole.MarkupLine($"[Red]Status Code: {ex.Status}[/]");
                AnsiConsole.MarkupLine($"[Red]Error Code: {ex.ErrorCode}[/]");
            }
        }

        return null;
    }
}
