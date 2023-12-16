﻿using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Text;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class Part2SettingsLoader
{
    public const string EnvironmentPrefix = "CODEMASH_SK_";

    public static Part2Settings? ExtractAndValidateSettings(IConfiguration config)
    {
        // Load settings
        string? key = config["OpenAI:Key"];
        string? endpoint = config["OpenAI:Endpoint"];
        string? textDeployment = config["OpenAI:TextDeployment"];
        string? chatDeployment = config["OpenAI:ChatDeployment"];
        string? embeddingDeployment = config["OpenAI:EmbeddingDeployment"];

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(endpoint))
        {
            StringBuilder sb = new($"The application is missing required configuration variables for OpenAI.{Environment.NewLine}" +
                               $"Check your [SteelBlue]appsettings.json[/] file and restart the application.{Environment.NewLine}{Environment.NewLine}");

            sb.AppendLine("Missing variables:");
            if (string.IsNullOrWhiteSpace(key))
            {
                sb.AppendLine($"- [Orange1]OpenAI:Key[/]");
            }
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                sb.AppendLine($"- [Orange1]OpenAI:Endpoint[/]");
            }

            sb.AppendLine();
            sb.AppendLine($"You can also set these variables via user secrets or environment variables prefixed by [SteelBlue]{EnvironmentPrefix}[/].");
            sb.AppendLine($"See [SteelBlue]README.md[/] for more instructions.");

            DisplayHelpers.DisplayBorderedMessage("Additional Configuration Needed", sb.ToString(), Color.Red);
            return null;
        }

        if (string.IsNullOrEmpty(textDeployment))
        {
            AnsiConsole.MarkupLine($"[Orange1]OpenAI:TextDeployment[/] is not set. Using text completions will be disabled.");
        }
        if (string.IsNullOrEmpty(chatDeployment))
        {
            AnsiConsole.MarkupLine($"[Orange1]OpenAI:ChatDeployment[/] is not set. Using chat completions will be disabled.");
        }
        if (string.IsNullOrEmpty(embeddingDeployment))
        {
            AnsiConsole.MarkupLine($"[Orange1]OpenAI:EmbeddingDeployment[/] is not set. Using embeddings will be disabled.");
        }

        DisplayHelpers.DisplayBorderedMessage("Part 2 OpenAI Setup Confirmed",
                                      "Your machine is configured and ready to go.",
                                      Color.Green);

        return new Part2Settings(key, endpoint, textDeployment, chatDeployment, embeddingDeployment);
    }   
}