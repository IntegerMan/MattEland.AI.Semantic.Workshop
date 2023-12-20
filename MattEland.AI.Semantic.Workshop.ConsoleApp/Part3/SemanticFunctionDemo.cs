﻿using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Spectre.Console;
using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public class SemanticFunctionDemo
{
    private readonly Part3Settings _settings;

    public SemanticFunctionDemo(Part3Settings settings)
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

        builder.Plugins.AddFromType<TimeInformation>();

        Kernel kernel = builder.Build();

        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            OpenAIPromptExecutionSettings executionSettings = new() {
                ChatSystemPrompt = "You are Alfred, Batman's virtual butler. You are a demo app at the CodeMash 2024 conference. The user is Batman. " +
                                   "Help the user with any questions they have in a professional, courteous, and wry manner.",
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            };

            FunctionResult response = await kernel.InvokePromptAsync(userText, new KernelArguments(executionSettings));
            string reply = response.ToString();

            // Completion / prompt tokens can be helpful, but can also be distracting, so read the setting
            if (_settings.ShowTokenUsage)
            {
                CompletionsUsage usage = (CompletionsUsage)response.Metadata!["Usage"]!;
                DisplayHelpers.DisplayTokenUsage(usage);
            }

            // Content filtering results aren't really helpful in most cases, so only show them if we're supposed to or if something hit the filter
            List<ContentFilterResultsForPrompt> filters = (List<ContentFilterResultsForPrompt>)response.Metadata!["PromptFilterResults"]!;
            if (_settings.ShowFilterResults || filters.IsContentFiltered())
            {
                DisplayHelpers.DisplayContentFilterResults(filters.First().ContentFilterResults);
            }

            AnsiConsole.MarkupLine($"[SteelBlue]Bot:[/] {reply}");
            AnsiConsole.WriteLine();

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}

public class TimeInformation
{
    [KernelFunction]
    [Description("Retrieves the current time in UTC.")]
    public string GetCurrentUtcTime() => DateTime.UtcNow.ToString("R");
}