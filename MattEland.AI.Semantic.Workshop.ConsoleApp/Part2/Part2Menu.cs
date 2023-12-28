using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Properties;
using System.Numerics.Tensors;
using Spectre.Console;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using System.Text.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class Part2Menu
{
    private readonly AppSettings _settings;
    private readonly LargeLanguageModelDemo _llm;
    private readonly ChatDemo _chat;
    private readonly ImageDemo _dalle;
    private readonly EmbeddingDemo _embeddings;

    public Part2Menu(AppSettings settings)
    {
        _settings = settings;
        _llm = new LargeLanguageModelDemo(_settings);
        _chat = new ChatDemo(_settings, Resources.ChatAssistantSystemPrompt);
        _dalle = new ImageDemo(_settings);
        _embeddings = new EmbeddingDemo(_settings);
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
            { "Batman and Robin presenting at CodeMash", () => "Batman and Robin presenting at a technical conference"},
            { "Board Game Extravaganza", () => "A bunch of programmers playing many different board games at a conference"},
            { "Bacon Buffet", () => "An illustration of programming conference attendees waiting in line at a buffet featuring bacon and only bacon at CodeMash 2024." },
            { "Custom text", () => AnsiConsole.Prompt<string>(new TextPrompt<string>("[Yellow]Enter your own image prompt:[/]")) },
            { "Back", () => string.Empty }
        };

        Dictionary<string, Func<string>> embeddingSources = new()
        {
            { "CodeMash Blog Post 1: Join us", () => Resources.CodeMashBlogPost1},
            { "CodeMash Blog Post 2: Maker Space", () => Resources.CodeMashBlogPost2},
            { "CodeMash Blog Post 3: KidzMash", () => Resources.CodeMashBlogPost3},
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
                    bool keepChatting;
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

                case Part2MenuOptions.GenerateTextEmbedding:
                    string embeddingChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                               .Title("What text do you want to generate embeddings for?")
                                               .HighlightStyle(Style.Parse("Orange3"))
                                               .AddChoices(embeddingSources.Keys)
                                               .UseConverter(c => c));

                    if (embeddingChoice == "Back")
                        break;

                    string embeddingPrompt = embeddingSources[embeddingChoice]();

                    float[] embeddings = await _embeddings.GetEmbeddingsAsync(embeddingPrompt);

                    AnsiConsole.MarkupLine($"[Yellow]Embedding Data:[/] {string.Join(", ", embeddings.ToArray())}");
                    break;

                case Part2MenuOptions.SearchEmbedding:
                    string searchPrompt = AnsiConsole.Prompt(new TextPrompt<string>("[Yellow]Enter the text to search for:[/]"));

                    float[] searchEmbeddings = await _embeddings.GetEmbeddingsAsync(searchPrompt);
                    List<ArticleLinkWithEmbeddings> searchableArticles = JsonSerializer.Deserialize<List<ArticleLinkWithEmbeddings>>(Resources.SearchableEmbeddings)!;

                    foreach (ArticleLinkWithEmbeddings article in searchableArticles)
                    {
                        double score = TensorPrimitives.CosineSimilarity(article.Embeddings, searchEmbeddings);
                        article.Score = score;
                    }

                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine($"[Yellow]Top Results:[/]");
                    foreach (ArticleLinkWithEmbeddings result in searchableArticles.OrderByDescending(a => a.Score).Take(5))
                    {
                        AnsiConsole.MarkupLine($"- [Yellow]Score:[/] {result.Score:F3}, [Yellow]Url:[/] [SteelBlue]{result.Url}[/]");
                    }
                    break;

                case Part2MenuOptions.Lab:
                    Part2Lab lab = new(_settings);
                    await lab.RunAsync();
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
