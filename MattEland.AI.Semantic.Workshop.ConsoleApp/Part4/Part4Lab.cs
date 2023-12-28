
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;

public class Part4Lab : KernelDemoBase
{
    public Part4Lab(AppSettings settings) : base(settings)
    {
    }

    public override async Task RunAsync()
    {
        // In this lab you're free to experiment with plugins. Try creating a plugin or two of your own and incorporating them into the kernel.
        // Some ideas:
        // - Add a plugin that returns a string with your own areas of technical interest and ask it what sessions would be good to attend (this will synergize nicely with the Sessionize plugin)
        // - Add a plugin that lists out restaurants in the area and asks for a recommendation. You can hard code these entries.
        // - Add a plugin that reads and possibly writes to a text file. Such a plugin could store TODO items, grocery lists, or other information.
        // - Add a plugin that calls out to Azure AI Language to analyze the input text and return information about it using Linked, PII, or Healthcare Entities as we saw in part 1.
        // - Add a plugin that incorporates a third party API you have familiarity with. It'll be helpful if there's an existing .NET client library for working with this API.

        // HINT: The Part4Lab class inherits from KernelDemoBase like our other Kernel examples do. You can take advantage of the helper methods this provides or ignore them and do your own thing.

        AnsiConsole.MarkupLine("[Yellow]Replace this with your implementation of this lab[/]");
        await Task.CompletedTask; // You can remove this once you have at least one await in your method.

        // EXTRA CREDIT: If you build something cool, show us!
    }
}