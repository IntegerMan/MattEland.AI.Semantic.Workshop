using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.Text;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;

public class Part1SettingsLoader
{
    public const string EnvironmentPrefix = "CODEMASH_SK_";

    public static Part1Settings? ExtractAndValidateSettings(IConfiguration config)
    {
        // Load settings
        string? aiKey = config["AzureAIServices:Key"];
        string? aiEndpoint = config["AzureAIServices:Endpoint"];
        string? aiRegion = config["AzureAIServices:Region"];

        if (string.IsNullOrWhiteSpace(aiKey) || string.IsNullOrWhiteSpace(aiEndpoint) || string.IsNullOrWhiteSpace(aiRegion))
        {
            StringBuilder sb = new($"The application is missing required configuration variables for Azure AI Services.{Environment.NewLine}" +
                               $"Check your [SteelBlue]appsettings.json[/] file and restart the application.{Environment.NewLine}{Environment.NewLine}");

            sb.AppendLine("Missing variables:");
            if (string.IsNullOrWhiteSpace(aiKey))
            {
                sb.AppendLine($"- [Orange1]AzureAIServices:Key[/]");
            }
            if (string.IsNullOrWhiteSpace(aiRegion))
            {
                sb.AppendLine($"- [Orange1]AzureAIServices:Region[/]");
            }
            if (string.IsNullOrWhiteSpace(aiEndpoint))
            {
                sb.AppendLine($"- [Orange1]AzureAIServices:Endpoint[/]");
            }

            sb.AppendLine();
            sb.AppendLine($"You can also set these variables via user secrets or environment variables prefixed by [SteelBlue]{EnvironmentPrefix}[/].");
            sb.AppendLine($"See [SteelBlue]README.md[/] for more instructions.");

            DisplayHelpers.DisplayBorderedMessage("Additional Configuration Needed", sb.ToString(), Color.Red);
            return null;
        }

        DisplayHelpers.DisplayBorderedMessage("Part 1 Azure AI Setup Confirmed",
                                      "Your machine is configured and ready to go.",
                                      Color.Green);

        return new Part1Settings(aiKey, aiEndpoint, aiRegion)
        {
            VoiceName = config["AzureAIServices:VoiceName"] ?? "en-GB-AlfieNeural"
        };
    }   
}
