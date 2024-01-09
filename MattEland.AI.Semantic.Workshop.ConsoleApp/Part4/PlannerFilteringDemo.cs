using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.OpenMeteo;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;

#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class PlannerFilteringDemo : KernelDemoBase
{
    public PlannerFilteringDemo(AppSettings settings) : base(settings)
    {
    }

    public override async Task RunAsync()
    {
        string[] weatherStrings = ["rain", "weather", "hot", "cold", "temperature", "snow"];
        string[] conferenceStrings = ["CodeMash", "conference", "session", "talk", "speaker", "KidzMash"];

        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);
        AddPlugins(builder);

        Kernel kernel = builder.Build();

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            // We're now creating a planner per request so we can configure what functions should be included
            FunctionCallingStepwisePlannerConfig plannerConfig = new();
            foreach (KernelPlugin plugin in kernel.Plugins)
            {
                // We can omit entire plugins if they're not relevant to the user's request
                if (plugin.Name == nameof(OpenMeteoPlugin) && !weatherStrings.Any(w => userText.Contains(w, StringComparison.OrdinalIgnoreCase)))
                {
                    AnsiConsole.MarkupLine($"[SteelBlue]{plugin.Name}[/] [Orange3]Excluded[/]: No weather-related text present");

                    plannerConfig.ExcludedPlugins.Add(plugin.Name);
                    continue;
                }
                else if (plugin.Name == nameof(SessionizePlugin) && !conferenceStrings.Any(s => userText.Contains(s, StringComparison.OrdinalIgnoreCase)))
                {
                    AnsiConsole.MarkupLine($"[SteelBlue]{plugin.Name}[/] [Orange3]Excluded[/]: No conference-related text present");

                    plannerConfig.ExcludedPlugins.Add(plugin.Name);
                    continue;
                }

                foreach (KernelFunction function in plugin)
                {
                    // We can also omit individual functions if they're not relevant to the user's request
                    if (function.Name == "SearchArticles" && !userText.Contains("article", StringComparison.OrdinalIgnoreCase))
                    {
                        AnsiConsole.MarkupLine($"[SteelBlue]{plugin.Name}[/]:[Yellow]{function.Name}[/] [Orange3]Excluded[/]: No article-related text present");
                        plannerConfig.ExcludedFunctions.Add(function.Name);
                        continue;
                    }

                    AnsiConsole.MarkupLine($"[SteelBlue]{plugin.Name}[/]:[Yellow]{function.Name}[/] [Green]Available[/]");
                }
            }

            FunctionCallingStepwisePlanner planner = new(plannerConfig);
            FunctionCallingStepwisePlannerResult result = await planner.ExecuteAsync(kernel, userText);
            string reply = result.FinalAnswer;

            AnsiConsole.WriteLine();

            await DisplayBotResponseAsync(reply);

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}

#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

