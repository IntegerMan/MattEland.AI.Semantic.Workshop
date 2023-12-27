using Azure.AI.OpenAI;
using Spectre.Console;
using Spectre.Console.Json;
using Spectre.Console.Rendering;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;

public static class DisplayHelpers
{
    public static void DisplayBorderedMessage(string header, string message)
    {
        DisplayBorderedMessage(header, message, Color.SteelBlue);
    }

    public static void DisplayBorderedMessage(string header, string message, Color borderColor)
    {
        AnsiConsole.Write(new Panel(message)
                       .Header(header)
                       .BorderStyle(new Style(borderColor))
                       .Expand());
        AnsiConsole.WriteLine();
    }

    public static void DisplayBorderedMessage(string header, IRenderable content)
    {
        DisplayBorderedMessage(header, content, Color.SteelBlue);
    }

    public static void DisplayBorderedMessage(string header, IRenderable content, Color borderColor)
    {
        AnsiConsole.Write(new Panel(content)
                       .Header(header)
                       .BorderStyle(new Style(borderColor))
                       .Expand());
        AnsiConsole.WriteLine();
    }

    public static string ToFriendlyName(this Enum value)
    {
        FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute[]? attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        
        if (attributes is null || attributes.Length == 0)
            return value.ToString();

        return attributes?[0].Description ?? value.ToString();
    }

    public static void DisplayImage(this string imagePath)
    {
        CanvasImage image = new(imagePath);
        image.MaxWidth = 30;
        AnsiConsole.Write(image);
    }

    public static async Task DisplayImageAsync(this Uri imageUri)
    {
        using HttpClient webClient = new();
        using Stream stream = await webClient.GetStreamAsync(imageUri);

        stream.DisplayImage();
    }

    public static void DisplayImage(this Stream imageStream)
    {
        CanvasImage image = new(imageStream);
        image.MaxWidth = 30;
        AnsiConsole.Write(image);
    }

    public static void DisplayObjectJson(this object source, string header = "JSON")
    {
        // Note: this demo app mixes Newtonsoft and system.text.json for conveniences in both / to work around default limitations of both
        // Here Newtonsoft is able to serialize Type objects by default while System.Text.Json isn't.
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(source);
        JsonText jsonText = new(json);
        AnsiConsole.Write(new Panel(jsonText)
                                .Header(header)
                                .BorderColor(Color.SteelBlue));
    }


    public static IRenderable GetContentFilterDisplay(ContentFilterResultDetailsForPrompt filter)
    {
        Table contentTable = new();
        contentTable.Title = new TableTitle("[Yellow]Content Filter Results[/]");

        contentTable.AddColumns("Type", "Severity", "Filtered?");
        AddContentFilterRow(contentTable, filter.Sexual, "Sexual");
        AddContentFilterRow(contentTable, filter.Violence, "Violence");
        AddContentFilterRow(contentTable, filter.SelfHarm, "Self-Harm");

        return contentTable;
    }

    private static void AddContentFilterRow(Table table, ContentFilterResult? result, string name)
    {
        // This can happen in some cases, so let's just not render anything
        if (result is null)
            return;

        string severity = result.Severity.ToString();

        severity = severity switch
        {
            "safe" => "[Green]Safe[/]",
            "low" => "[Yellow]Low[/]",
            "medium" => "[Orange3]Medium[/]",
            "high" => "[Red]High[/]",
            _ => severity
        };

        table.AddRow($"[SteelBlue]{name}[/]", severity, result.Filtered ? "[Red]Yes[/]" : "[Green]No[/]");
    }

    public static void DisplayTokenUsage(CompletionsUsage usage)
    {
        AnsiConsole.Write(GetTokenUsageDisplay(usage));
        AnsiConsole.WriteLine();
    }

    public static IRenderable GetTokenUsageDisplay(CompletionsUsage usage)
    {
        BreakdownChart chart = new BreakdownChart()
                    .FullSize()
                    .AddItem("Prompt", usage.PromptTokens, Color.Yellow)
                    .AddItem("Completion", usage.CompletionTokens, Color.SteelBlue);

        return new Panel(chart)
                       .Header($"[White]Token Usage ({usage.TotalTokens} Tokens)[/]")
                       .BorderStyle(new Style(Color.SteelBlue))
                       .Expand();
    }
}
