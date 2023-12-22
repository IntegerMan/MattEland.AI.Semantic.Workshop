using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public class Part3Menu
{
    private readonly AppSettings _settings;

    public Part3Menu(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        KernelDemoBase? demo = null;
        do
        {
            Part3MenuOptions choice = AnsiConsole.Prompt(new SelectionPrompt<Part3MenuOptions>()
                .Title("What task in part 3?")
                .HighlightStyle(Style.Parse("Orange3"))
                .AddChoices(Enum.GetValues(typeof(Part3MenuOptions)).Cast<Part3MenuOptions>())
                .UseConverter(c => c.ToFriendlyName()));

            demo = choice switch
            {
                Part3MenuOptions.SimpleChat => new SimpleKernelDemo(_settings),
                Part3MenuOptions.SimpleChatWithTemplate => new TemplatedChatDemo(_settings),
                Part3MenuOptions.Classification => new ClassificationDemo(_settings),
                Part3MenuOptions.ChainedFunctions => new ChainedDemo(_settings),
                _ => null
            };

            if (demo is not null)
            {
                await demo.RunAsync();
            }

            AnsiConsole.WriteLine();
        } while (demo != null);

        await Task.CompletedTask;
    }
}
