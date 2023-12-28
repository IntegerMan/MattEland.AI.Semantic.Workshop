using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public class MainMenu
{
    private readonly AppSettings _settings;

    public MainMenu(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        // Standard cost disclaimer
        if (!_settings.SkipCostDisclaimer)
        {
            DisplayHelpers.DisplayBorderedMessage("Cost Disclaimer",
                                                  "[Yellow]This workshop uses Azure AI Services and OpenAI / Azure OpenAI. These services incur a per-call charge to work with. Nether the presenters nor the conference organizers are not responsible for any charges you incur.[/]",
                                                  Color.Red);
        }

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
                    // Run the submenu for Part 1
                    Part1Menu p1Menu = new(_settings.AzureAIServices);
                    await p1Menu.RunAsync();
                    break;
                case WorkshopMenuOption.Part2:
                    // Run the submenu for Part 2
                    Part2Menu p2Menu = new(_settings);
                    await p2Menu.RunAsync();
                    break;
                case WorkshopMenuOption.Part3:
                    Part3Menu p3Menu = new(_settings);
                    await p3Menu.RunAsync();
                    break;
                case WorkshopMenuOption.Part4:
                    Part4Menu p4Menu = new(_settings);
                    await p4Menu.RunAsync();
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
