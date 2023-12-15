using Azure.AI.Vision;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.AI.Vision.Common;
using Spectre.Console;
using System.Net;
using System.Text;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public class ImageAnalysisDemo
{
    private readonly VisionServiceOptions _serviceOptions;

    public ImageAnalysisDemo(string endpoint, string key)
    {
        AzureKeyCredential credential = new(key);
        _serviceOptions = new VisionServiceOptions(endpoint, credential);
    }

    public async Task AnalyzeAsync(string imageSource)
    {
        VisionSource source;
        if (imageSource.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            source = VisionSource.FromUrl(new Uri(imageSource));

            // Download the image to a local file so we can show a preview of it
            using HttpClient webClient = new();
            using Stream stream = await webClient.GetStreamAsync(imageSource);

            CanvasImage image = new(stream);
            image.MaxWidth = 30;
            AnsiConsole.Write(image);
        }
        else
        {
            if (!File.Exists(imageSource))
            {
                AnsiConsole.MarkupLine($"[Red]File not found: {imageSource}[/]");
                return;
            }

            source = VisionSource.FromFile(imageSource);

            CanvasImage image = new(imageSource);
            image.MaxWidth = 30;
            AnsiConsole.Write(image);
        }

        ImageAnalysisOptions analysisOptions = new()
        {
            GenderNeutralCaption = false,
            Language = "en",
            Features = ImageAnalysisFeature.Caption |
                       ImageAnalysisFeature.Tags |
                       ImageAnalysisFeature.Objects |
                       ImageAnalysisFeature.People |
                       ImageAnalysisFeature.DenseCaptions |
                       ImageAnalysisFeature.Text
        };

        using ImageAnalyzer client = new(_serviceOptions, source, analysisOptions);

        ImageAnalysisResult result = await client.AnalyzeAsync();

        // Captions
        AnsiConsole.MarkupLine($"[Yellow]Caption:[/] {result.Caption.Content} ({result.Caption.Confidence:P} Confidence)");

        // Tags
        if (result.Tags.Count > 0)
        {
            AnsiConsole.MarkupLine($"[Yellow]Tags:[/] {string.Join(", ", result.Tags.Select(t => t.Name))}");
        }
        else
        {
            AnsiConsole.MarkupLine($"[Yellow]Tags:[/] No tags detected");
        }

        // Object Detection
        if (result.Objects.Count > 0)
        {
            StringBuilder sb = new();
            foreach (DetectedObject obj in result.Objects)
            {
                sb.AppendLine($"[Yellow]Object:[/] {obj.Name} ({obj.Confidence:P} Confidence) at {obj.BoundingBox}");
            }
            AnsiConsole.MarkupLine(sb.ToString());
        }
        else
        {
            AnsiConsole.MarkupLine($"[Yellow]No objects detected[/]");
        }

        // People
        if (result.People.Count > 0)
        {
            StringBuilder sb = new();
            foreach (DetectedPerson person in result.People)
            {
                sb.AppendLine($"[Yellow]Person[/] ({person.Confidence:P} Confidence) at {person.BoundingBox}");
            }
            AnsiConsole.MarkupLine(sb.ToString());
        }
        else
        {
            AnsiConsole.MarkupLine($"[Yellow]No people detected[/]");
        }

        // Dense Captions
        if (result.DenseCaptions.Count > 0)
        {
            StringBuilder sb = new();
            foreach (ContentCaption caption in result.DenseCaptions)
            {
                sb.AppendLine($"[Yellow]Dense Caption:[/] {caption.Content} ({caption.Confidence:P} Confidence) at {caption.BoundingBox}");
            }
            AnsiConsole.MarkupLine(sb.ToString());
        }
        else
        {
            AnsiConsole.MarkupLine($"[Yellow]No dense captions detected[/]");
        }

        // Text
        if (result.Text.Lines.Count > 0)
        {
            StringBuilder sb = new();
            foreach (DetectedTextLine line in result.Text.Lines)
            {
                sb.AppendLine($"[Yellow]Text:[/] {line.Content}");
            }
            AnsiConsole.MarkupLine(sb.ToString());
        }
        else
        {
            AnsiConsole.MarkupLine($"[Yellow]No text detected[/]");
        }

    }
}