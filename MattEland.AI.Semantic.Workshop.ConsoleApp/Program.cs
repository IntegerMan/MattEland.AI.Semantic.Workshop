using System;
using Spectre.Console;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using MattEland.AI.Semantic.Workshop.ConsoleApp;

// Using UTF8 allows more capabilities for Spectre.Console.
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Display the header
AnsiConsole.Write(new FigletText("CodeMash AI").Color(Color.Yellow));
AnsiConsole.MarkupLine("Precompiler workshop led by [SteelBlue]Matt Eland[/] and [SteelBlue]Sam Gomez[/].");
AnsiConsole.WriteLine();

// Load settings
const string EnvironmentPrefix = "CODEMASH_SK_";
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables(EnvironmentPrefix)
    .Build();

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
            Part1Settings? p1Settings = Part1SettingsLoader.ExtractAndValidateSettings(config, EnvironmentPrefix);
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

AnsiConsole.WriteLine("Thanks for checking out the workshop!");
