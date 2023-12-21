using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class HandlebarsPlannerDemo : KernelDemoBase
{
    public HandlebarsPlannerDemo(Part3Settings settings) : base(settings)
    {
    }

    public async Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);

        builder.Plugins.AddFromType<TimePlugin>();

        Kernel kernel = builder.Build();
        kernel.FunctionInvoked += OnFunctionInvoked;
        kernel.FunctionInvoking += OnFunctionInvoking;
        kernel.PromptRendering += OnPromptRendering;
        kernel.PromptRendered += OnPromptRendered;

        HandlebarsPlannerOptions plannerConfig = new();
        HandlebarsPlanner planner = new(plannerConfig);

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            HandlebarsPlan plan = await planner.CreatePlanAsync(kernel, userText);
            string reply = await plan.InvokeAsync(kernel);

            AnsiConsole.MarkupLine($"[SteelBlue]Bot:[/] {reply}");
            AnsiConsole.WriteLine();

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}

#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


