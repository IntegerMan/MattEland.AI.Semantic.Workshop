using Azure;
using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class ImageDemo
{
    private readonly OpenAIClient _client;

    public ImageDemo(AppSettings settings)
    {
        // DALL-E on Azure OpenAI is not yet supported. We'll use the OpenAI API instead.
        _client = new OpenAIClient(settings.OpenAI.Key);
    }

    public async Task GenerateImageAsync(string prompt)
    {
        ImageGenerationOptions options = new()
        {
            Prompt = prompt,
            // In the future, with Azure OpenAI, we'll be able to use the DALL-E model by specifying a deployment name here
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
