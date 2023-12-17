using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public class Part3Menu
{
    private readonly Part3Settings _settings;

    public Part3Menu(Part3Settings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();

        if (string.IsNullOrEmpty(_settings.OpenAiEndpoint))
        {
            builder.AddOpenAIChatCompletion(_settings.ChatDeployment!, _settings.OpenAiKey);
        }
        else
        {
            builder.AddAzureOpenAIChatCompletion(_settings.ChatDeployment!, _settings.OpenAiEndpoint, _settings.OpenAiKey);
        }

        Kernel kernel = builder.Build();

        FunctionResult response = await kernel.InvokePromptAsync("Hello, I am a chatbot. What is your name?");

        string reply = response.ToString();
        CompletionsUsage usage = (CompletionsUsage)response.Metadata!["Usage"]!;
        List<ContentFilterResultsForPrompt> filters = (List<ContentFilterResultsForPrompt>)response.Metadata["PromptFilterResults"]!;
        ContentFilterResultDetailsForPrompt filter = filters.First().ContentFilterResults;

        AnsiConsole.MarkupLine($"[Yellow]Bot:[/] {reply}");

        AnsiConsole.MarkupLine($"[Yellow]Usage:[/] {usage.TotalTokens} tokens: {usage.PromptTokens} prompt tokens and {usage.CompletionTokens} completion tokens");
        AnsiConsole.MarkupLine($"[Yellow]Filter:[/] {filter.Sexual.Severity} Sexual, {filter.SelfHarm.Severity} Self-Harm, {filter.Violence.Severity} Violence");
    }
}
