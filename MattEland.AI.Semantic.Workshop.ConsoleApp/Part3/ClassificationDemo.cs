using Azure.AI.OpenAI;
using Azure.Core;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public class ClassificationDemo : KernelDemoBase
{
    public ClassificationDemo(AppSettings settings) : base(settings)
    {
    }

    public override async Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);

        Kernel kernel = builder.Build();
        KernelFunction getIntent = kernel.CreateFunctionFromPrompt(
                        new()
                        {
                            Template = @"
                            <message role=""system"">Instructions: Is the user talking about a batman villain or some food?
                            Do not explain the reasoning, just reply back with either villain or food. If you are unsure, reply with {{choices[0]}}.
                            Choices: {{choices}}.</message>

                            {{#each fewShotExamples}}
                                {{#each this}}
                                    <message role=""{{role}}"">{{content}}</message>
                                {{/each}}
                            {{/each}}

                            {{#each chatHistory}}
                                <message role=""{{role}}"">{{content}}</message>
                            {{/each}}

                            <message role=""user"">{{request}}</message>
                            <message role=""system"">Classification:</message>",
                            TemplateFormat = "handlebars"
                        },
                        new HandlebarsPromptTemplateFactory()
                    );

        // Create choices
        List<string> choices = ["Villain", "Food"];

        // Create few-shot examples
        List<ChatHistory> fewShotExamples = [
            [
                new ChatMessageContent(AuthorRole.User, "I saw the Riddler speaking about Probability at the Hard Rock Casino"),
                new ChatMessageContent(AuthorRole.System, "Classification:"),
                new ChatMessageContent(AuthorRole.Assistant, "Villain")
            ],
            [
                new ChatMessageContent(AuthorRole.User, "I had the fish at dinner last night. Not bad."),
                new ChatMessageContent(AuthorRole.System, "Classification:"),
                new ChatMessageContent(AuthorRole.Assistant, "Food")
            ]
                ];

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            FunctionResult response = await kernel.InvokeAsync(
            getIntent,
                    new() {
                        { "request", userText },
                        { "choices", choices },
                        { "fewShotExamples", fewShotExamples }
            }
        );
            RenderMetadata(response.Metadata, "Response Metadata");
            
            string reply = response.ToString();

            await DisplayBotResponseAsync(reply);

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}
