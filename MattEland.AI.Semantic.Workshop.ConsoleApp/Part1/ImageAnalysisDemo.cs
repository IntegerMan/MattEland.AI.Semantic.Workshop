using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.AI.Vision.Common;
using Spectre.Console;
using System.Text;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;

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
            Uri uri = new Uri(imageSource);
            source = VisionSource.FromUrl(uri);

            // Show a web image
            await DisplayHelpers.DisplayImageAsync(uri);
        }
        else
        {
            if (!File.Exists(imageSource))
            {
                AnsiConsole.MarkupLine($"[Red]File not found: {imageSource}[/]");
                return;
            }

            source = VisionSource.FromFile(imageSource);

            DisplayHelpers.DisplayImage(imageSource);
        }

        ImageAnalysisOptions analysisOptions = new()
        {
            GenderNeutralCaption = false,
            Language = "en",
            CroppingAspectRatios = [1.5],
            Features = ImageAnalysisFeature.Caption |
                       ImageAnalysisFeature.Tags |
                       ImageAnalysisFeature.Objects |
                       ImageAnalysisFeature.People |
                       ImageAnalysisFeature.DenseCaptions |
                       ImageAnalysisFeature.Text |
                       ImageAnalysisFeature.CropSuggestions
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

        // Crop Suggestions
        if (result.CropSuggestions.Count > 0)
        {
            StringBuilder sb = new();
            foreach (CropSuggestion suggestion in result.CropSuggestions)
            {
                sb.AppendLine($"[Yellow]Crop Suggestion:[/] Aspect Ratio: {suggestion.AspectRatio}, Bounds: {suggestion.BoundingBox}");
            }
            AnsiConsole.MarkupLine(sb.ToString());
        }
        else
        {
            AnsiConsole.MarkupLine($"[Yellow]No crop suggestions detected[/]");
        }
    }

    public async Task RemoveBackgroundAsync(string imageSource)
    {
        VisionSource source;
        if (imageSource.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            Uri uri = new Uri(imageSource);
            source = VisionSource.FromUrl(uri);

            // Show a web image
            await DisplayHelpers.DisplayImageAsync(uri);
        }
        else
        {
            if (!File.Exists(imageSource))
            {
                AnsiConsole.MarkupLine($"[Red]File not found: {imageSource}[/]");
                return;
            }

            source = VisionSource.FromFile(imageSource);

            DisplayHelpers.DisplayImage(imageSource);
        }

        ImageAnalysisOptions analysisOptions = new()
        {
            SegmentationMode = ImageSegmentationMode.ForegroundMatting,
            Features = ImageAnalysisFeature.None
        };

        using ImageAnalyzer client = new(_serviceOptions, source, analysisOptions);

        ImageAnalysisResult result = await client.AnalyzeAsync();

        File.WriteAllBytes("ForegroundMatte.png", result.SegmentationResult.ImageBuffer.ToArray());

        DisplayHelpers.DisplayImage("ForegroundMatte.png");
    }
}
