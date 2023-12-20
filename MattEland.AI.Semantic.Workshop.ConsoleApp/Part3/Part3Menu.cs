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


        bool keepChatting;
        do
        {
            string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
            AnsiConsole.WriteLine();

            FunctionResult response = await kernel.InvokePromptAsync(userText);
            string reply = response.ToString();

            CompletionsUsage usage = (CompletionsUsage)response.Metadata!["Usage"]!;
            DisplayHelpers.DisplayTokenUsage(usage);

            List<ContentFilterResultsForPrompt> filters = (List<ContentFilterResultsForPrompt>)response.Metadata["PromptFilterResults"]!;
            DisplayHelpers.DisplayContentFilterResults(filters.First().ContentFilterResults);

            AnsiConsole.MarkupLine($"[SteelBlue]Bot:[/] {reply}");
            AnsiConsole.WriteLine();

            keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
            AnsiConsole.WriteLine();
        } while (keepChatting);
    }
}
