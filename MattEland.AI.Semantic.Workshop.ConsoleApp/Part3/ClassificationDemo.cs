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

    public async override Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();
        AddLargeLanguageModelIntegration(builder);

        Kernel kernel = builder.Build();
        KernelFunction getIntent = kernel.CreateFunctionFromPrompt(
                        new()
                        {
                            Template = @"
                            <message role=""system"">Instructions: What is the intent of this request?
                            Do not explain the reasoning, just reply back with the intent. If you are unsure, reply with {{choices[0]}}.
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
                            <message role=""system"">Intent:</message>",
                            TemplateFormat = "handlebars"
                        },
                        new HandlebarsPromptTemplateFactory()
                    );

        // Create choices
        List<string> choices = ["ContinueConversation", "EndConversation"];

        // Create few-shot examples
        List<ChatHistory> fewShotExamples = [
            [
                new ChatMessageContent(AuthorRole.User, "Can you send a very quick approval to the marketing team?"),
                new ChatMessageContent(AuthorRole.System, "Intent:"),
                new ChatMessageContent(AuthorRole.Assistant, "ContinueConversation")
            ],
            [
                new ChatMessageContent(AuthorRole.User, "Thanks, I'm done for now"),
                new ChatMessageContent(AuthorRole.System, "Intent:"),
                new ChatMessageContent(AuthorRole.Assistant, "EndConversation")
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

            DisplayBotResponse(reply);

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}
