using Azure.AI.OpenAI;
using Azure.Core;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public class TemplatedChatDemo : KernelDemoBase
{
    public TemplatedChatDemo(Part3Settings settings) : base(settings)
    {
    }

    public async override Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);

        Kernel kernel = builder.Build();
        KernelFunction chatFunc = kernel.CreateFunctionFromPrompt(new()
                        {
                            Template = @"<message role=""system"">Instructions: You are Alfred, the virtual AI assistant to Batman.
                            You are chatting with Batman. Be polite, helpful, and have a wry sense of humor.
                            Answer his questions and help him out to the best of your ability.</message>
                            <message role=""user"">{{request}}</message>",
                            TemplateFormat = "handlebars"
                        },
                        new HandlebarsPromptTemplateFactory()
                    );

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            FunctionResult response = await kernel.InvokeAsync(chatFunc, new() { { "request", userText } } );
            RenderMetadata(response.Metadata, "Response Metadata");

            string reply = response.ToString();

            AnsiConsole.MarkupLine($"[SteelBlue]Bot:[/] {reply}");
            AnsiConsole.WriteLine();

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}
