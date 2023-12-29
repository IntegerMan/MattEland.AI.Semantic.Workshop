using Azure;
using Azure.AI.OpenAI;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class Part2LabSolution
{
    private readonly AppSettings _settings;

    public Part2LabSolution(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        // In this lab you'll have Azure AI Services analyze an image to generate a caption.
        // You'll then incorporate this caption into a prompt to generate a new image using DALL-E.
        // This will effectively let you generate new images based on what an AI model sees in an image.

        // Replace this with your own image if you want
        string imagePath = "https://lh3.googleusercontent.com/pw/ABLVV84QNUe_lPC-28or7JFEoYvq6M7npX-rVJceV_WbJ8VBd9nymn25sa2WhwpM2JJ4EhbOAhK3XpZqZpmrTI00r2u1bv4EBega7BBdMO2-3ztuhM0a0Ydm7SobNjIGmr6Ayqwz9eAY415Cvs7W03Rh2C_gtw=w1232-h924-s-no?authuser=0";
        Uri imageUri = new Uri(imagePath);

        AnsiConsole.MarkupLine($"[Yellow]Image to Analyze[/]");
        await DisplayHelpers.DisplayImageAsync(imageUri);

        // Analyze the image using Azure AI Services to generate a caption
        AzureKeyCredential credential = new(_settings.AzureAIServices.Key);
        VisionServiceOptions serviceOptions = new(_settings.AzureAIServices.Endpoint, credential);
        using VisionSource source = VisionSource.FromUrl(imageUri);

        ImageAnalysisOptions analysisOptions = new()
        {
            GenderNeutralCaption = true,
            Language = "en",
            Features = ImageAnalysisFeature.Caption
        };

        using ImageAnalyzer client = new(serviceOptions, source, analysisOptions);

        ImageAnalysisResult captionResult = await client.AnalyzeAsync();

        // Display the caption
        AnsiConsole.MarkupLine($"[Yellow]Caption:[/] {Markup.Escape(captionResult.Caption.Content)}");

        // Use the caption as a prompt for DALL-E to generate a new image
        OpenAIClient dalleClient = new(_settings.OpenAI.Key);

        ImageGenerationOptions options = new()
        {
            Prompt = captionResult.Caption.Content,
        };

        Response<ImageGenerations> imageResult = await dalleClient.GetImageGenerationsAsync(options);
        Uri generatedUrl = imageResult.Value.Data.First().Url;

        // Display the generated image
        AnsiConsole.MarkupLine($"[Yellow]Generated Image[/]");
        await DisplayHelpers.DisplayImageAsync(generatedUrl);
    }
}