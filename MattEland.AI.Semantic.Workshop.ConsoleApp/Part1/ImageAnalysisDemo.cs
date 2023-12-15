using Azure.AI.Vision;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.AI.Vision.Common;
using Spectre.Console;
using System.Net;

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
            Features = ImageAnalysisFeature.Caption
        };

        using ImageAnalyzer client = new(_serviceOptions, source, analysisOptions);

        ImageAnalysisResult result = await client.AnalyzeAsync();

        AnsiConsole.MarkupLine($"[Yellow]Caption:[/] {result.Caption.Content} ({result.Caption.Confidence:P} Confidence)");
    }
}