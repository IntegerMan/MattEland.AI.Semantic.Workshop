using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.OpenMeteo;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;

#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class PlannerPersonalityDemo : KernelDemoBase
{
    public PlannerPersonalityDemo(AppSettings settings) : base(settings)
    {
    }

    public async override Task RunAsync()
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

