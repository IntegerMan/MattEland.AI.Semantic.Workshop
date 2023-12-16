using MattEland.AI.Semantic.Workshop.ConsoleApp.Properties;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class Part2Menu
{
    private readonly Part2Settings _settings;
    private readonly LargeLanguageModelDemo _llm;
    private readonly ChatDemo _chat;
    private readonly ImageDemo _dalle;

    public Part2Menu(Part2Settings p2Settings)
    {
        _settings = p2Settings;
        _llm = new LargeLanguageModelDemo(_settings);
        _chat = new ChatDemo(_settings, Resources.ChatAssistantSystemPrompt);
        _dalle = new ImageDemo(_settings);
    }

    public async Task RunAsync()
    {
        Dictionary<string, Func<string>> textSources = new()
        {
            { "Zero Shot Inference Example", () => Resources.TextZeroShot},
            { "One Shot Inference Example", () => Resources.TextOneShot},
            { "Few Shot Inference Example", () => Resources.TextFewShot},
            { "Custom text", () => AnsiConsole.Prompt<string>(new TextPrompt<string>("[Yellow]Enter your own text prompt:[/]")) },
            { "Back", () => string.Empty }
        };

        Dictionary<string, Func<string>> imageSources = new()
        {
            { "Disapproving Gorilla", () => "An oil painting of disapproving gorilla staring at the viewer"},
            { "Batman and Robin presenting at CodeMash", () => "Batman presenting at a technical conference with Robin there helping"},
            { "Bacon Buffet", () => "An illustration of programming conference attendees waiting in line at a buffet featuring bacon and only bacon at CodeMash 2024." },
            { "Custom text", () => AnsiConsole.Prompt<string>(new TextPrompt<string>("[Yellow]Enter your own image prompt:[/]")) },
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

                    bool keepChatting = true;
                    do
                    {
                        string userText = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]You:[/]"));
                        string? chatResponse = await _chat.GetChatCompletionAsync(userText);

                        if (chatResponse is not null)
                        {
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine($"[SteelBlue]Bot:[/] {Markup.Escape(chatResponse)}");
                        }

                        AnsiConsole.WriteLine();
                        keepChatting = AnsiConsole.Confirm("Keep chatting?", true);
                        AnsiConsole.WriteLine();
                    } while (keepChatting);

                    break;

                case Part2MenuOptions.ImageCompletion:
                    if (string.IsNullOrEmpty(_settings.ImageDeployment))
                    {
                        AnsiConsole.MarkupLine($"[Red]No image deployment specified. Please check your settings.[/]");
                        break;
                    }

                    string imageChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                               .Title("What image prompt do you want to use?")
                                               .HighlightStyle(Style.Parse("Orange3"))
                                               .AddChoices(imageSources.Keys)
                                               .UseConverter(c => c));

                    if (imageChoice == "Back")
                        break;

                    string imagePrompt = imageSources[imageChoice]();
                    AnsiConsole.MarkupLine($"[Yellow]Image Prompt:[/] [SteelBlue]{Markup.Escape(imagePrompt)}[/]");

                    await _dalle.GenerateImageAsync(imagePrompt);
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
