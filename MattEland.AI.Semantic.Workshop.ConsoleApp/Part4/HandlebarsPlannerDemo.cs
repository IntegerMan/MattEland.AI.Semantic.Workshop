using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;

#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class HandlebarsPlannerDemo : KernelDemoBase
{
    public HandlebarsPlannerDemo(AppSettings settings) : base(settings)
    {
    }

    public async override Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);
        AddPlugins(builder);

        Kernel kernel = builder.Build();
        kernel.FunctionInvoked += OnFunctionInvoked;
        kernel.PromptRendered += OnPromptRendered;

        HandlebarsPlannerOptions plannerConfig = new()
        {
            AllowLoops = true,
        };
        HandlebarsPlanner planner = new(plannerConfig);

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            HandlebarsPlan plan = await planner.CreatePlanAsync(kernel, userText);
            AnsiConsole.MarkupLine($"[Yellow]Plan:[/] {plan}");

            string reply = await plan.InvokeAsync(kernel);

            await DisplayBotResponseAsync(reply);

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}

#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


