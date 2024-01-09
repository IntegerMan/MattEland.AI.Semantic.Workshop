using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
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

    public override async Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);
        AddPlugins(builder);

        Kernel kernel = builder.Build();
        kernel.FunctionInvoked += OnFunctionInvoked;
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

            // Display details on the plan for diagnostics:
            if (result.ChatHistory is not null)
            {
                foreach (ChatMessageContent step in result.ChatHistory)
                {
                    if (step.Content is not null)
                    {
                        if (step.Role == AuthorRole.User)
                        {
                            AnsiConsole.MarkupLine($"[Yellow]Prompt:[/] {step.Content}");
                        }
                        else if (step.Role == AuthorRole.Assistant)
                        {
                            AnsiConsole.MarkupLine($"[SteelBlue]Reasoning:[/] {step.Content}");
                        } 
                        else
                        {
                            AnsiConsole.MarkupLine($"[Orange3]{step.Role}:[/] {step.Content}");
                        }
                    }
                    if (step.Metadata is not null)
                    {
                        RenderMetadata(step.Metadata, $"{step.Role} Metadata");
                    }
                    AnsiConsole.WriteLine();
                }
            }

            AnsiConsole.WriteLine();

            await DisplayBotResponseAsync(reply);

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}

#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
