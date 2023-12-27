using System;
using Spectre.Console;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MattEland.AI.Semantic.Workshop.ConsoleApp;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;

try
{

    // Using UTF8 allows more capabilities for Spectre.Console.
    Console.OutputEncoding = Encoding.UTF8;
    Console.InputEncoding = Encoding.UTF8;

    // Display the header
    AnsiConsole.Write(new FigletText("CodeMash AI").Color(Color.Yellow));
    AnsiConsole.MarkupLine("Precompiler workshop led by [SteelBlue]Matt Eland[/] and [SteelBlue]Sam Gomez[/].");
    AnsiConsole.WriteLine();

    // Standard cost disclaimer
    DisplayHelpers.DisplayBorderedMessage("Cost Disclaimer",
                                          "[Yellow]This workshop uses Azure AI Services and OpenAI / Azure OpenAI. These services incur a per-call charge to work with. Nether the presenters nor the conference organizers are not responsible for any charges you incur.[/]",  
                                          Color.Red);

    // Load settings
    const string EnvironmentPrefix = Part1SettingsLoader.EnvironmentPrefix;
    IConfigurationRoot config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddUserSecrets(Assembly.GetExecutingAssembly())
        .AddEnvironmentVariables(EnvironmentPrefix)
        .Build();

    MainMenu menu = new(config);
    await menu.RunAsync();

    AnsiConsole.WriteLine("Thanks for checking out the workshop!");
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    AnsiConsole.WriteLine();
    AnsiConsole.WriteLine("Press any key to exit.");
    Console.ReadKey();
}