using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Microsoft.SemanticKernel;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public class SimpleKernelDemo : KernelDemoBase
{
    public SimpleKernelDemo(Part3Settings settings) : base(settings)
    {
    }

    public async override Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);

        Kernel kernel = builder.Build();

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            FunctionResult response = await kernel.InvokePromptAsync(userText);
            RenderMetadata(response.Metadata, "Response Metadata");

            string reply = response.ToString();

            AnsiConsole.MarkupLine($"[SteelBlue]Bot:[/] {reply}");
            AnsiConsole.WriteLine();

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}
