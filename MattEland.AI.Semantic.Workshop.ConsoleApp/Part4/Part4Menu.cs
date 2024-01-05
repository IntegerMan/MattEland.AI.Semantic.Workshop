using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;

public class Part4Menu
{
    private readonly AppSettings _settings;

    public Part4Menu(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        KernelDemoBase? demo = null;
        do
        {
            IEnumerable<Part4MenuOptions> choices = Enum.GetValues(typeof(Part4MenuOptions))
                .Cast<Part4MenuOptions>()
                .Where(o => !o.GetType().GetField(o.ToString())!
                    .IsDefined(typeof(HiddenAttribute), false));

            Part4MenuOptions choice = AnsiConsole.Prompt(new SelectionPrompt<Part4MenuOptions>()
                .Title("What task in part 4?")
                .HighlightStyle(Style.Parse("Orange3"))
                .AddChoices(choices)
                .UseConverter(c => c.ToFriendlyName()));

            demo = choice switch
            {
                Part4MenuOptions.PluginDemo => new PluginDemo(_settings),
                Part4MenuOptions.KernelEvents => new EventsDemo(_settings),
                Part4MenuOptions.HandlebarsPlanner => new HandlebarsPlannerDemo(_settings),
                Part4MenuOptions.FunctionCallingPlanner => new FunctionCallingStepwisePlannerDemo(_settings),
                Part4MenuOptions.PlannerFilteringDemo => new PlannerFilteringDemo(_settings),
                Part4MenuOptions.Lab => new Part4Lab(_settings),
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
