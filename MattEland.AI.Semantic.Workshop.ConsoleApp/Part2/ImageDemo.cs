using Azure;
using Azure.AI.OpenAI;
using Spectre.Console;
using System;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class ImageDemo
{
    private readonly Part2Settings _settings;
    private readonly OpenAIClient _client;

    public ImageDemo(Part2Settings settings)
    {
        _settings = settings;

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
    }

    public async Task GenerateImageAsync(string prompt)
    {
        ImageGenerationOptions options = new()
        {
            DeploymentName = _settings.ImageDeployment,
            //Size = ImageSize.Size256x256,
            Prompt = prompt,
        };

        try
        {
            // Get the response from OpenAI
            Response<ImageGenerations> result = await _client.GetImageGenerationsAsync(options);

            // Create a temp file with this image data in the application directory
            Guid guid = Guid.NewGuid();
            string tempFile = Path.Combine(AppContext.BaseDirectory, $"{guid}.png");
            Uri url = result.Value.Data.First().Url;
            AnsiConsole.MarkupLine($"[Yellow]Image URL:[/] {Markup.Escape(url.ToString())}");

            // Display the image
            await DisplayHelpers.DisplayImageAsync(url);
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
                AnsiConsole.MarkupLine($"[Red]Operation not supported. You may need to deploy a different model. ??? is known to work.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[Red]Error: {ex.Message} ({ex.GetType().Name})[/]");
                AnsiConsole.MarkupLine($"[Red]Status Code: {ex.Status}[/]");
                AnsiConsole.MarkupLine($"[Red]Error Code: {ex.ErrorCode}[/]");
            }
        }
    }
}
