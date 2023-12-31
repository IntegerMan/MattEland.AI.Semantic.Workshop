using Spectre.Console;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MattEland.AI.Semantic.Workshop.ConsoleApp;

try
{

    // Using UTF8 allows more capabilities for Spectre.Console.
    Console.OutputEncoding = Encoding.UTF8;
    Console.InputEncoding = Encoding.UTF8;

    // Display the header
    AnsiConsole.Write(new FigletText("CodeMash AI").Color(Color.Yellow));
    AnsiConsole.MarkupLine("Precompiler workshop led by [SteelBlue]Matt Eland[/] and [SteelBlue]Sam Gomez[/].");
    AnsiConsole.WriteLine();

    // Load settings
    IConfigurationRoot config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddUserSecrets(Assembly.GetExecutingAssembly())
        .AddEnvironmentVariables("CODEMASH_SK_") // if you define values in memory as environment variables, they must start with this. E.G. CODEMASH_SK_SkipCostDisclaimer or CODEMASH_SK_AzureAIServices:Key
        .Build();

    AppSettings settings = config.Get<AppSettings>()!;


    // Run the menu system
    MainMenu menu = new(settings);
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