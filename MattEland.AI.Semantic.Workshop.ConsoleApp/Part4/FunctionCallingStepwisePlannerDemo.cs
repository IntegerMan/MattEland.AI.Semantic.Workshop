using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;

#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class FunctionCallingStepwisePlannerDemo : KernelDemoBase
{
    public FunctionCallingStepwisePlannerDemo(AppSettings settings) : base(settings)
    {
    }

    public async override Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);

        builder.Plugins.AddFromType<TimePlugin>();

        Kernel kernel = builder.Build();
        kernel.FunctionInvoked += OnFunctionInvoked;
        kernel.FunctionInvoking += OnFunctionInvoking;
        kernel.PromptRendering += OnPromptRendering;
        kernel.PromptRendered += OnPromptRendered;

        FunctionCallingStepwisePlannerConfig plannerConfig = new();
        FunctionCallingStepwisePlanner planner = new(plannerConfig);

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            FunctionCallingStepwisePlannerResult result = await planner.ExecuteAsync(kernel, userText);
            string reply = result.FinalAnswer;

            AnsiConsole.MarkupLine($"[SteelBlue]Bot:[/] {reply}");
            AnsiConsole.WriteLine();

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}

#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

