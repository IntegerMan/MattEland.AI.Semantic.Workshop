using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Properties;
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
        DisplayConfigurationStatus();

        // Standard cost disclaimer
        if (!_settings.SkipCostDisclaimer)
        {
            DisplayHelpers.DisplayBorderedMessage("Cost Disclaimer", Resources.CostDisclaimerMessage, Color.Red);
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
                    if (_settings.AzureAIServices.IsConfigured)
                    {
                        // Run the submenu for Part 1
                        Part1Menu p1Menu = new(_settings.AzureAIServices);
                        await p1Menu.RunAsync();
                    }
                    else
                    {
                        DisplayHelpers.DisplayBorderedMessage("Azure AI Services not configured", Resources.AzureAIServicesNotConfiguredMessage, Color.Orange3);
                    }
                    break;
                case WorkshopMenuOption.Part2:
                    if (_settings.OpenAI.IsConfigured || _settings.AzureOpenAI.IsConfigured)
                    {
                        // Run the submenu for Part 2
                        Part2Menu p2Menu = new(_settings);
                        await p2Menu.RunAsync();
                    }
                    else
                    {
                        DisplayHelpers.DisplayBorderedMessage("OpenAI not configured", Resources.OpenAINotConfiguredMessage, Color.Orange3);
                    }
                    break;
                case WorkshopMenuOption.Part3:
                    if (_settings.OpenAI.IsConfigured || _settings.AzureOpenAI.IsConfigured)
                    {
                        Part3Menu p3Menu = new(_settings);
                        await p3Menu.RunAsync();
                    }
                    else
                    {
                        DisplayHelpers.DisplayBorderedMessage("OpenAI not configured", Resources.OpenAINotConfiguredMessage, Color.Orange3);
                    }
                    break;
                case WorkshopMenuOption.Part4:
                    if (_settings.OpenAI.IsConfigured || _settings.AzureOpenAI.IsConfigured)
                    {
                        Part4Menu p4Menu = new(_settings);
                        await p4Menu.RunAsync();
                    }
                    else
                    {
                        DisplayHelpers.DisplayBorderedMessage("OpenAI not configured", Resources.OpenAINotConfiguredMessage, Color.Orange3);
                    }
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

    private void DisplayConfigurationStatus()
    {
        // Display the Azure AI Services status
        if (_settings.AzureAIServices.IsConfigured)
        {
            AnsiConsole.MarkupLine($"[Green]:check_mark_button:Azure AI Services are configured. You're ready for part 1![/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[Orange3]:yellow_square:Azure AI Services are not configured. You will not be able to do part 1 or the part 2 lab![/]");
            DisplayHelpers.DisplayBorderedMessage("Azure AI Services not configured", Resources.AzureAIServicesNotConfiguredMessage, Color.Orange3);
        }

        // Display the OpenAI Services status
        if (_settings.AzureOpenAI.IsConfigured && !_settings.AzureOpenAI.IsDisabled)
        {
            AnsiConsole.MarkupLine($"[Green]:check_mark_button:[Blue]Azure OpenAI[/] is configured. You're ready for parts 2, 3, & 4![/]");
        }
        else if (_settings.OpenAI.IsConfigured)
        {
            AnsiConsole.MarkupLine($"[Green]:check_mark_button:OpenAI is configured. You're ready for parts 2, 3, & 4![/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[Orange3]:yellow_square:OpenAI is not configured. You will not be able to do parts 2, 3, or 4![/]");
            DisplayHelpers.DisplayBorderedMessage("OpenAI is not configured", Resources.OpenAINotConfiguredMessage, Color.Orange3);
        }

        AnsiConsole.WriteLine();
    }
}
