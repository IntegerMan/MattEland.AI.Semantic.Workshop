using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.Sessionize;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;
using Microsoft.SemanticKernel;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

#pragma warning disable SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public abstract class KernelDemoBase
{
    protected KernelDemoBase(AppSettings settings)
    {
        Settings = settings;
    }

    protected void AddLargeLanguageModelIntegration(IKernelBuilder builder)
    {
        if (string.IsNullOrEmpty(Settings.OpenAiEndpoint))
        {
            builder.AddOpenAIChatCompletion(Settings.ChatDeployment!, Settings.OpenAiKey);
        }
        else
        {
            builder.AddAzureOpenAIChatCompletion(Settings.ChatDeployment!, Settings.OpenAiEndpoint, Settings.OpenAiKey);
        }
    }

    protected void AddPlugins(IKernelBuilder builder)
    {
        builder.Plugins.AddFromType<TimePlugin>();

        if (!string.IsNullOrEmpty(Settings.SessionizeApiToken))
        {
            builder.Plugins.AddFromObject(new SessionizePlugin(Settings.SessionizeApiToken));
        }
    }

    public abstract Task RunAsync();

    protected AppSettings Settings { get; }

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
        AnsiConsole.WriteLine(e.RenderedPrompt);

        RenderMetadata(e.Metadata, $"{e.Function.Name} Rendered Metadata");
    }

    protected static void RenderMetadata(IReadOnlyDictionary<string, object?>? metadata, string title)
    {
        if (metadata is not null && metadata.Count > 0)
        {
            Table table = new();
            table.Title = new TableTitle($"[SteelBlue]{Markup.Escape(title)}[/]");
            table.AddColumns("Key", "Value");
            foreach (string key in metadata.Keys)
            {
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
                    table.AddRow(new Text(Markup.Escape(key)), DisplayHelpers.GetContentFilterDisplay(filterList.First().ContentFilterResults));
                }
                else
                {
                    table.AddRow(Markup.Escape(key), Markup.Escape(value.ToString()!));
                }
            }
            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
        }
    }
}
#pragma warning restore SKEXP0004 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
