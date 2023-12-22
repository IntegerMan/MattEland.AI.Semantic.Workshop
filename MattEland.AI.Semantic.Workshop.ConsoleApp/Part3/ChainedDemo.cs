using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public class ChainedDemo : KernelDemoBase
{
    public ChainedDemo(AppSettings settings) : base(settings)
    {
    }

    public async override Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);

        Kernel kernel = builder.Build();
        KernelFunction batFunc = kernel.CreateFunctionFromPrompt(
                new()
                {
                    Template = @"
                            <message role=""system"">Instructions: You are Batman. Respond to the user as if you are Batman.</message>

                            {{#each chatHistory}}
                                <message role=""{{role}}"">{{content}}</message>
                            {{/each}}

                            <message role=""user"">{{request}}</message>
                            <message role=""system""></message>",
                    TemplateFormat = "handlebars"
                },
                new HandlebarsPromptTemplateFactory()
            );
        KernelFunction alfredFunc = kernel.CreateFunctionFromPrompt(
                new()
                {
                    Template = @"
                            <message role=""system"">Instructions: You are Alfred, the virtual AI assistant to Batman. Summarize whatever input you get as if you are explaining it to Batman. Be polite, helpful, and have a wry sense of humor.</message>

                            {{#each chatHistory}}
                                <message role=""{{role}}"">{{content}}</message>
                            {{/each}}

                            <message role=""user"">{{request}}</message>
                            <message role=""system""></message>",
                    TemplateFormat = "handlebars"
                },
                new HandlebarsPromptTemplateFactory()
            );

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            // Invoke the Batman function with userText
            FunctionResult batmanResponse = await batFunc.InvokeAsync(kernel, new() { { "request", userText } });
            RenderMetadata(batmanResponse.Metadata, "Batman Response Metadata");

            AnsiConsole.MarkupLine($"[SteelBlue]Batman:[/] {batmanResponse}");

            FunctionResult alfredResponse = await alfredFunc.InvokeAsync(kernel, new() { { "request", batmanResponse.ToString() } });
            RenderMetadata(alfredResponse.Metadata, "Alfred Response Metadata");

            AnsiConsole.MarkupLine($"[SteelBlue]Alfred:[/] {alfredResponse}");
            AnsiConsole.WriteLine();

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}
