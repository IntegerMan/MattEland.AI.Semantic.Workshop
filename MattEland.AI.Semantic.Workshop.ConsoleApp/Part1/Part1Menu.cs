using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public class Part1Menu
{
    private readonly TextAnalysisDemo _textAnalysis;

    public Part1Menu(Part1Settings settings)
    {
        _textAnalysis = new TextAnalysisDemo(settings.AiEndpoint, settings.AiKey);
    }

    public async Task RunAsync()
    {
        Dictionary<string, Func<string>> textSources = new()
        {
            { "Custom text", () => AnsiConsole.Prompt<string>(new TextPrompt<string>("[Yellow]Enter your own text to analyze:[/]")) },
            { "This Workshop's Abstract", () => Properties.Resources.WorkshopAbstract},
            { "Semantic Kernel Announcement", () => Properties.Resources.SemanticKernelAnnouncement },
        };

        bool hasQuit = false;
        while (!hasQuit)
        {
            Part1MenuOptions choice = AnsiConsole.Prompt(new SelectionPrompt<Part1MenuOptions>()
                .Title("What task in part 1?")
                .HighlightStyle(Style.Parse("Orange3"))
                .AddChoices(Enum.GetValues(typeof(Part1MenuOptions)).Cast<Part1MenuOptions>())
                .UseConverter(c => c.ToFriendlyName()));

            switch (choice)
            {
                case Part1MenuOptions.AnalyzeText:
                    string textToAnalyze = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                               .Title("What text do you want to analyze?")
                                               .HighlightStyle(Style.Parse("Orange3"))
                                               .AddChoices(textSources.Keys)
                                               .UseConverter(c => c));

                    AnsiConsole.MarkupLine($"[Yellow]Analyzing {textToAnalyze}[/]");
                    string documentText = textSources[textToAnalyze]();

                    await _textAnalysis.AnalyzeAsync(documentText);
                    break;

                case Part1MenuOptions.AnalyzeImage:
                    AnsiConsole.WriteLine("Image analysis is not yet implemented. Please check back later.");
                    break;

                case Part1MenuOptions.TextToSpeech:
                    AnsiConsole.WriteLine("Text to speech is not yet implemented. Please check back later.");
                    break;

                case Part1MenuOptions.SpeechToText:
                    AnsiConsole.WriteLine("Speech to text is not yet implemented. Please check back later.");
                    break;

                case Part1MenuOptions.Back:
                    hasQuit = true;
                    break;

                default:
                    AnsiConsole.WriteLine($"Matt apparently forgot to handle menu choice {choice}. What a dolt!");
                    break;
            }

            AnsiConsole.WriteLine();
        }
    }
}
