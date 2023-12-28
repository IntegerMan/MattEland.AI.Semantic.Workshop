
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class Part2Lab
{
    private readonly AppSettings _settings;

    public Part2Lab(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        // In this lab you'll have Azure AI Services analyze an image to generate a caption.
        // You'll then incorporate this caption into a prompt to generate a new image using DALL-E.
        // This will effectively let you generate new images based on what an AI model sees in an image.

        // HINT: Use DisplayHelpers.DisplayImageAsync with the URL of your image to show a pixelated version here, and control click to navigate to its URL from the application

        // If you get stuck, reach out for help, or look at Part2LabSolution.cs for a working solution.

        AnsiConsole.MarkupLine("[Yellow]Replace this with your implementation of this lab[/]");
        await Task.CompletedTask; // You can remove this once you have at least one await in your method.

        // EXTRA CREDIT:
        // If you finish early, try using OpenAI to customize your image prompt so the flow goes from Azure AI Services analyzing the image to OpenAI customizing the prompt to DALL-E generating an image.
    }
}