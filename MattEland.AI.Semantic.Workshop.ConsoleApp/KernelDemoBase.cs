using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;
using Microsoft.SemanticKernel;
using Spectre.Console;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.OpenMeteo;
using System.Text;
using Spectre.Console.Json;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public abstract class KernelDemoBase
{
    private SpeechDemo _speech;

    protected KernelDemoBase(AppSettings settings)
    {
        Settings = settings;
    }

    protected void AddLargeLanguageModelIntegration(IKernelBuilder builder)
    {
        bool useAzureOpenAI = !string.IsNullOrEmpty(Settings.AzureOpenAI.Endpoint);
        if (useAzureOpenAI)
        {
            builder.AddAzureOpenAIChatCompletion(Settings.AzureOpenAI.ChatDeploymentName, Settings.AzureOpenAI.Endpoint, Settings.AzureOpenAI.Key);
            builder.AddAzureOpenAITextEmbeddingGeneration(Settings.AzureOpenAI.EmbeddingDeploymentName, Settings.AzureOpenAI.Endpoint, Settings.AzureOpenAI.Key);
        }
        else
        {
            builder.AddOpenAIChatCompletion(Settings.OpenAI.ChatModel, Settings.OpenAI.Key);
            builder.AddOpenAITextEmbeddingGeneration(Settings.OpenAI.EmbeddingModel, Settings.OpenAI.Key);
        }
    }

    protected void AddPlugins(IKernelBuilder builder)
    {
        builder.Plugins.AddFromType<TimePlugin>();
        builder.Plugins.AddFromType<OpenMeteoPlugin>();
        builder.Plugins.AddFromType<PreferencesPlugin>();
        builder.Plugins.AddFromType<EmbeddingSearchPlugin>();

        if (!string.IsNullOrEmpty(Settings.SessionizeApiToken))
        {
            builder.Plugins.AddFromObject(new SessionizePlugin(Settings.SessionizeApiToken));
        }
    }       

    public abstract Task RunAsync();

    protected AppSettings Settings { get; }

    public async Task DisplayBotResponseAsync(string reply)
    {
        DisplayHelpers.DisplayBorderedMessage("Alfred", Markup.Escape(reply), Color.SteelBlue);
        AnsiConsole.WriteLine();

        // Speak this message if configured to in the settings
        if (Settings.AzureOpenAI.IsConfigured && Settings.SpeakKernelResponses) 
        { 
            _speech ??= new SpeechDemo(Settings.AzureAIServices.Region, Settings.AzureAIServices.Key, Settings.AzureAIServices.VoiceName);

            await _speech.SpeakAsync(reply);
        }
    }

    protected void OnFunctionInvoking(object? sender, FunctionInvokingEventArgs e)
    {
        AnsiConsole.MarkupLine($"[Yellow]Function Invoking:[/] {e.Function.Name}");

        RenderMetadata(e.Metadata, $"{e.Function.Name} Invoking Metadata");
    }

    protected void OnFunctionInvoked(object? sender, FunctionInvokedEventArgs e)
    {
        AnsiConsole.MarkupLine($"[Yellow]Function Invoked:[/] {Markup.Escape(e.Function.Name)}, result: {Markup.Escape(e.Result.ToString())}");

        RenderMetadata(e.Metadata, $"{e.Function.Name} Invoked Metadata");
    }

    protected void OnPromptRendering(object? sender, PromptRenderingEventArgs e)
    {
        AnsiConsole.MarkupLine($"[Yellow]Prompt Rendering:[/] {Markup.Escape(e.Function.Name)}");

        RenderMetadata(e.Metadata, $"{e.Function.Name} Rendering Metadata");
    }

    protected void OnPromptRendered(object? sender, PromptRenderedEventArgs e)
    {
        AnsiConsole.MarkupLine($"[Yellow]Prompt Rendered:[/] {Markup.Escape(e.Function.Name)}");

        // Some responses have a [FUNCTIONS] section that we want to render as JSON. This represents the JSON contents of our available functions and is more readable using Spectre's JSON rendering
        int functionsIndex = e.RenderedPrompt.IndexOf("[FUNCTIONS]", StringComparison.Ordinal);
        int endFunctionsIndex = e.RenderedPrompt.IndexOf("[END FUNCTIONS]", StringComparison.Ordinal);
        if (functionsIndex > -1 && endFunctionsIndex > -1 && endFunctionsIndex > functionsIndex)
        {
            AnsiConsole.WriteLine(e.RenderedPrompt.Substring(0, functionsIndex));
            string json = e.RenderedPrompt.Substring(functionsIndex + "[FUNCTIONS]".Length, endFunctionsIndex - functionsIndex - "[FUNCTIONS]".Length);
            json = json.Trim();
            AnsiConsole.Write(new JsonText(json));
            AnsiConsole.WriteLine(e.RenderedPrompt.Substring(endFunctionsIndex + "[END FUNCTIONS]".Length));
        }
        else
        {
            AnsiConsole.WriteLine(e.RenderedPrompt);
        }

        RenderMetadata(e.Metadata, $"{e.Function.Name} Rendered Metadata");
    }

    protected static void RenderMetadata(IReadOnlyDictionary<string, object?>? metadata, string title)
    {
        if (metadata is not null && metadata.Count > 0)
        {
            int rows = 0;

            Table table = new();
            table.Title = new TableTitle($"[SteelBlue]{Markup.Escape(title)}[/]");
            table.AddColumns("Key", "Value");

            // Loop over each column and render it based on its type
            foreach (string key in metadata.Keys)
            {
                // These keys don't generally have helpful information, so let's skip them
                if (key == "SystemFingerprint" || key == "Created" || key == "Id" || key.EndsWith(".Id"))
                    continue;

                object? value = metadata[key];
                if (value?.ToString() is null)
                {
                    table.AddRow(Markup.Escape(key), "[Orange3]<null>[/]");
                }
                else if (value is CompletionsUsage usage)
                {
                    table.AddRow(new Text(Markup.Escape(key)), DisplayHelpers.GetTokenUsageDisplay(usage));
                }
                else if (value is List<ContentFilterResultsForPrompt> filterList)
                {
                    // In some cases we see null results come back, so let's not even render those in the UI
                    if (filterList.Count == 0 || filterList.First().ContentFilterResults.Sexual is null)
                    {
                        continue;
                    }

                    table.AddRow(new Text(Markup.Escape(key)), DisplayHelpers.GetContentFilterDisplay(filterList.First().ContentFilterResults));
                }
                else if (value is List<ChatCompletionsFunctionToolCall> toolCalls && toolCalls.Any())
                {
                    StringBuilder sbCalls = new("Calls were made to the following functions:");
                    foreach (var call in toolCalls)
                    {
                        sbCalls.AppendLine($"- {call.Name} ({call.Arguments})");
                    }
                    table.AddRow(Markup.Escape(key), Markup.Escape(sbCalls.ToString()));
                }
                else
                {
                    table.AddRow(Markup.Escape(key), Markup.Escape(value.ToString()!));
                }

                rows++;
            }

            // In case we got a useless table, don't render it
            if (rows > 0)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Write(table);
            }
        }
    }
}
#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.