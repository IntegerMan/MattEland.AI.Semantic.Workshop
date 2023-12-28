
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;

public class Part1Lab
{
    private readonly AzureAISettings _settings;

    public Part1Lab(AzureAISettings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        // In this lab, you'll use Azure AI Services to listen to words spoken into the microphone, analyze them with Azure AI Language, and then speak a summary back to the user.
        // It's up to you what aspects of Azure AI Language you use, but key phrase extraction, abstractive summarization, and sentiment analysis are good ideas to consider.        
        // You should need to create a speech client as well as a language client in order to accomplish this task. Use values from _settings to accomplish this.

        // If you get stuck, reach out for help, or look at Part1LabSolution.cs for a working solution.
        AnsiConsole.MarkupLine("[Yellow]Replace this with your implementation of this lab[/]");
        await Task.CompletedTask; // You can remove this once you have at least one await in your method.

        // EXTRA CREDIT: Try using sentiment analysis to generate different types of responses based on the overall sentiment.
    }
}