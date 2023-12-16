using Azure;
using Azure.AI.OpenAI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class LargeLanguageModelDemo
{
    private readonly OpenAIClient _client;
    private readonly Part2Settings _settings;

    public LargeLanguageModelDemo(Part2Settings settings)
    {
        if (string.IsNullOrEmpty(settings.OpenAiEndpoint))
        {
            _client = new OpenAIClient(settings.OpenAiKey);
        } 
        else
        {
            Uri uri = new(settings.OpenAiEndpoint);
            AzureKeyCredential key = new(settings.OpenAiKey);
            _client = new OpenAIClient(uri, key);
        }

        _settings = settings;
    }

    public async Task<string?> GetTextCompletionAsync(string prompt)
    {
        try
        {
            CompletionsOptions options = new()
            {
                ChoicesPerPrompt = 1,
                MaxTokens = 100,
                DeploymentName = _settings.TextDeployment,
                Temperature = 1f, // ranges from 0 to 2
                Echo = false,
            };
            options.Prompts.Add(prompt);

            Response<Completions> result = await _client.GetCompletionsAsync(options);

            return result.Value.Choices.First().Text;
        }
        catch (RequestFailedException ex)
        {
            if (ex.ErrorCode == "DeploymentNotFound")
            {
                AnsiConsole.MarkupLine($"[Red]Deployment {_settings.TextDeployment} not found. Please check your settings.[/]");
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
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[Red]Error: {ex.Message} ({ex.GetType().Name})[/]");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }

        return null;
    }
}
