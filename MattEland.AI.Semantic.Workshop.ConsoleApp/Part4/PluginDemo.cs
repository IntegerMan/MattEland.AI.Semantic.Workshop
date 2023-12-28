using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;

public class PluginDemo : KernelDemoBase
{
    public PluginDemo(AppSettings settings) : base(settings)
    {
    }

    public async override Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);
        AddPlugins(builder);

        Kernel kernel = builder.Build();

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            OpenAIPromptExecutionSettings executionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            };

            FunctionResult response = await kernel.InvokePromptAsync(userText, new KernelArguments(executionSettings));

            RenderMetadata(response.Metadata, "Response Metadata");

            string reply = response.ToString();

            await DisplayBotResponseAsync(reply);

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}
