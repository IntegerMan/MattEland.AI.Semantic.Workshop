using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;
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
                        // Run the submenu for Part 1
                        Part1Menu p1Menu = new(p1Settings);
                        await p1Menu.RunAsync();
                    }
                    break;
                case WorkshopMenuOption.Part2:
                    Part2Settings? p2Settings = Part2SettingsLoader.ExtractAndValidateSettings(_config);
                    if (p2Settings is not null)
                    {
                        // Run the submenu for Part 2
                        Part2Menu p2Menu = new(p2Settings);
                        await p2Menu.RunAsync();
                    }
                    break;
                case WorkshopMenuOption.Part3:
                    Part3Settings? p3Settings = Part3SettingsLoader.ExtractAndValidateSettings(_config);
                    if (p3Settings is not null)
                    {
                        Part3Menu p3Menu = new(p3Settings);
                        await p3Menu.RunAsync();
                    }
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
