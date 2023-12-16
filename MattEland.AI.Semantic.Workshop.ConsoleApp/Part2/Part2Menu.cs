using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
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

    public Part2Menu(Part2Settings p2Settings)
    {
        _settings = p2Settings;
    }

    public async Task RunAsync()
    {
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
                    AnsiConsole.WriteLine("Text Completion is not yet implemented. Please check back later.");
                    break;

                case Part2MenuOptions.ChatCompletion:
                    AnsiConsole.WriteLine("Chat Completion is not yet implemented. Please check back later.");
                    break;

                case Part2MenuOptions.TextEmbedding:
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
