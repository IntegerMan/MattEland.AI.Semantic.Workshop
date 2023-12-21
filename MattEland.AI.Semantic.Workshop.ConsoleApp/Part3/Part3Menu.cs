using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics.Tensors;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;
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
        bool hasQuit = false;
        while (!hasQuit)
        {
            Part3MenuOptions choice = AnsiConsole.Prompt(new SelectionPrompt<Part3MenuOptions>()
                .Title("What task in part 3?")
                .HighlightStyle(Style.Parse("Orange3"))
                .AddChoices(Enum.GetValues(typeof(Part3MenuOptions)).Cast<Part3MenuOptions>())
                .UseConverter(c => c.ToFriendlyName()));

            switch (choice)
            {
                case Part3MenuOptions.SimpleChat:
                    SimpleKernelDemo simpleKernel = new(_settings);
                    await simpleKernel.RunAsync();
                    break;
                case Part3MenuOptions.SemanticFunction:
                    SemanticFunctionDemo functionDemo = new(_settings);
                    await functionDemo.RunAsync();
                    break;
                case Part3MenuOptions.KernelEvents:
                    EventsDemo eventsDemo = new(_settings);
                    await eventsDemo.RunAsync();
                    break;
                case Part3MenuOptions.FunctionCallingPlanner:
                    PlannerDemo plannerDemo = new(_settings);
                    await plannerDemo.RunAsync();
                    break;
                case Part3MenuOptions.ChainedFunctions:
                    AnsiConsole.WriteLine("Chained Functions not yet implemented. Please check back later.");
                    break;
                case Part3MenuOptions.Back:
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
