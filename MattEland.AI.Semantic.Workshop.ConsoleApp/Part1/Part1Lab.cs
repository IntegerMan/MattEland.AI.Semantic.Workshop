using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.AI.Vision.Common;
using Spectre.Console;
using System.Text;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Microsoft.CognitiveServices.Speech;
using Azure.AI.TextAnalytics;

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

        // HINT: You will need to create a SpeechConfig, SpeechRecognizer, SpeechSynthesizer, and TextAnalyticsClient.

        // If you get stuck, reach out for help, or look at Part1LabSolution.cs for a working solution.

        AnsiConsole.MarkupLine("[Yellow]Replace this with your implementation of this lab[/]");

        // Recognize speech
        AnsiConsole.MarkupLine("[Yellow]Speak your sentence to be analyzed[/]");

        string text = ""; // Replace this with the results from Azure
        AnsiConsole.MarkupLine($"[Yellow]Recognized:[/] {Markup.Escape(text)}");

        // Extract key phrases

        // HINT: You may choose to use a different method than AnalyzeActions if you only want one type of analysis. For example, ExtractKeyPhrasesAsync

        // Speak the key phrases
        string voicePrompt = $"The key phrases in your sentence are: "; // Add the key phrases here or whatever else you want to highlight
        AnsiConsole.MarkupLine($"[Yellow]Speaking:[/] {Markup.Escape(voicePrompt)}");

        // EXTRA CREDIT: Add error handling for the speech recognition, text analysis, and speech synthesizer as needed.
        // EXTRA CREDIT: Try using sentiment analysis to generate different types of responses based on the overall sentiment.

        await Task.CompletedTask; // Remove this line once you have an await keyword in your code. This just suppresses a warning from no async operations
    }
}