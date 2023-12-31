﻿using Azure;
using Azure.AI.OpenAI;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class Part2Lab
{
    private readonly AppSettings _settings;

    public Part2Lab(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        // In this lab you'll have Azure AI Services analyze an image to generate a caption.
        // You'll then incorporate this caption into a prompt to generate a new image using DALL-E.
        // This will effectively let you generate new images based on what an AI model sees in an image.

        // If you get stuck, reach out for help, or look at Part2LabSolution.cs for a working solution.

        AnsiConsole.MarkupLine("[Yellow]Replace this with your implementation of this lab[/]");

        // Replace this with your own image if you want
        string imagePath = "https://lh3.googleusercontent.com/pw/ABLVV84QNUe_lPC-28or7JFEoYvq6M7npX-rVJceV_WbJ8VBd9nymn25sa2WhwpM2JJ4EhbOAhK3XpZqZpmrTI00r2u1bv4EBega7BBdMO2-3ztuhM0a0Ydm7SobNjIGmr6Ayqwz9eAY415Cvs7W03Rh2C_gtw=w1232-h924-s-no?authuser=0";
        Uri imageUri = new(imagePath);

        AnsiConsole.MarkupLine($"[Yellow]Image to Analyze[/]");
        await DisplayHelpers.DisplayImageAsync(imageUri);

        // Analyze the image using Azure AI Services to generate a caption

        // Hint: You'll need an AzureKeyCredential, VisionServiceOptions, VisionSource, ImageAnalysisOptions, and ImageAnalyzer her
        // e
        string caption = ""; // Replace with something from Azure here

        // Display the caption
        AnsiConsole.MarkupLine($"[Yellow]Caption:[/] {Markup.Escape(caption)}");

        // Use the caption as a prompt for DALL-E to generate a new image

        // Hint: You'll need an OpenAIClient and ImageGenerationOptions here

        Uri? generatedUrl = null; // Replace with something from OpenAI here

        // Display the generated image
        AnsiConsole.MarkupLine($"[Yellow]Generated Image[/]");
        await DisplayHelpers.DisplayImageAsync(generatedUrl);

        // EXTRA CREDIT:
        // Try adding in more context beyond just the caption. Perhaps including tags or detected objects.
        // Try adding in a prefix suffix to your prompt to customize the style for example "An oil painting of {caption}"
        // If you finish early, try using OpenAI to customize your image prompt so the flow goes from Azure AI Services analyzing the image to OpenAI customizing the prompt to DALL-E generating an image.
    }
}