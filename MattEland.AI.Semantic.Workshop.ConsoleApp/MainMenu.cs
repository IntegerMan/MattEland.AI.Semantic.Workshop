using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public class MainMenu
{
    private readonly IConfiguration _config;

    public MainMenu(IConfiguration config)
    {
        _config = config;
    }

    public async Task RunAsync()
    {
        bool hasQuit = false;
        while (!hasQuit)
        {
            // Prompt for the workshop portion to run
            WorkshopMenuOption choice = AnsiConsole.Prompt(new SelectionPrompt<WorkshopMenuOption>()
                .Title("Which workshop portion do you want to run?")
                .HighlightStyle(Style.Parse("Orange3"))
                .AddChoices(Enum.GetValues(typeof(WorkshopMenuOption)).Cast<WorkshopMenuOption>())
                .UseConverter(c => c.ToFriendlyName()));

            switch (choice)
            {
                case WorkshopMenuOption.Part1:
                    Part1Settings? p1Settings = Part1SettingsLoader.ExtractAndValidateSettings(_config);
                    if (p1Settings is not null)
                    {
                        TextAnalysisDemo demo = new(p1Settings.AiEndpoint, p1Settings.AiKey);
                        await demo.AnalyzeSentimentAsync();
                    }
                    break;
                case WorkshopMenuOption.Part2:
                    AnsiConsole.WriteLine("Part 2 is not yet implemented. Please check back later.");
                    break;
                case WorkshopMenuOption.Part3:
                    AnsiConsole.WriteLine("Part 3 is not yet implemented. Please check back later.");
                    break;
                case WorkshopMenuOption.Part4:
                    AnsiConsole.WriteLine("Part 4 is not yet implemented. Please check back later.");
                    break;
                case WorkshopMenuOption.Quit:
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
