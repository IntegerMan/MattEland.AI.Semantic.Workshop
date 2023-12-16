﻿using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class Part2Menu
{
    private readonly Part2Settings _settings;
    private readonly LargeLanguageModelDemo _llm;

    public Part2Menu(Part2Settings p2Settings)
    {
        _settings = p2Settings;
        _llm = new LargeLanguageModelDemo(_settings);
    }

    public async Task RunAsync()
    {
        Dictionary<string, Func<string>> textSources = new()
        {
            { "Zero Shot Inference Example", () => Properties.Resources.TextZeroShot},
            { "One Shot Inference Example", () => Properties.Resources.TextOneShot},
            { "Few Shot Inference Example", () => Properties.Resources.TextFewShot},
            { "Custom text", () => AnsiConsole.Prompt<string>(new TextPrompt<string>("[Yellow]Enter your own text prompt:[/]")) },
            { "Back", () => string.Empty }
        };

        bool hasQuit = false;
        while (!hasQuit)
        {
            Part2MenuOptions choice = AnsiConsole.Prompt(new SelectionPrompt<Part2MenuOptions>()
                .Title("What task in part 2?")
                .HighlightStyle(Style.Parse("Orange3"))
                .AddChoices(Enum.GetValues(typeof(Part2MenuOptions)).Cast<Part2MenuOptions>())
                .UseConverter(c => c.ToFriendlyName()));

            switch (choice)
            {
                case Part2MenuOptions.TextCompletion:
                    if (string.IsNullOrEmpty(_settings.TextDeployment))
                    {
                        AnsiConsole.MarkupLine($"[Red]No text deployment specified. Please check your settings.[/]");
                        break;
                    }

                    string textChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                               .Title("What text prompt do you want to use?")
                                               .HighlightStyle(Style.Parse("Orange3"))
                                               .AddChoices(textSources.Keys)
                                               .UseConverter(c => c));

                    if (textChoice == "Back")
                        break;

                    string prompt = textSources[textChoice]();
                    AnsiConsole.Markup($"[Yellow]Text Prompt:[/] [SteelBlue]{Markup.Escape(prompt)}[/]");

                    string? response = await _llm.GetTextCompletionAsync(prompt);
                    if (response is not null)
                    {
                        AnsiConsole.WriteLine(response);
                    } 
                    AnsiConsole.WriteLine();
                    break;

                case Part2MenuOptions.ChatCompletion:
                    if (string.IsNullOrEmpty(_settings.ChatDeployment))
                    {
                        AnsiConsole.MarkupLine($"[Red]No chat deployment specified. Please check your settings.[/]");
                        break;
                    }

                    AnsiConsole.WriteLine("Chat Completion is not yet implemented. Please check back later.");
                    break;

                case Part2MenuOptions.TextEmbedding:
                    if (string.IsNullOrEmpty(_settings.EmbeddingDeployment))
                    {
                        AnsiConsole.MarkupLine($"[Red]No text embedding deployment specified. Please check your settings.[/]");
                        break;
                    }

                    AnsiConsole.WriteLine("Text Embedding is not yet implemented. Please check back later.");
                    break;

                case Part2MenuOptions.Back:
                    hasQuit = true;
                    break;

                default:
                    AnsiConsole.WriteLine($"Matt apparently forgot to handle menu choice {choice}. What a dolt!");
                    break;
            }

            AnsiConsole.WriteLine();
        }

        await Task.CompletedTask;
    }
}
