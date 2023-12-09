using System;
using Spectre.Console;
using System.Text;

// Using UTF8 allows more capabilities for Spectre.Console.
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// TODO: Read these values from a configuration file, with environment variables and user secrets as a backup

// Get the environment variables we need to run the workshop
// NOTE: If you do not want to use environment variables, replace Environment.GetEnvironmentVariable with a string literal.
string? openAiEndpoint = Environment.GetEnvironmentVariable("CODEMASH_SK_AZURE_OPENAI_ENDPOINT");
string? openAiKey = Environment.GetEnvironmentVariable("CODEMASH_SK_AZURE_OPENAI_KEY");
string? aiKey = Environment.GetEnvironmentVariable("CODEMASH_SK_AZURE_AI_KEY");
string? aiEndpoint = Environment.GetEnvironmentVariable("CODEMASH_SK_AZURE_AI_ENDPOINT");
string? aiRegion = Environment.GetEnvironmentVariable("CODEMASH_SK_AZURE_AI_REGION");

bool isReady = false;
string message;

if (string.IsNullOrWhiteSpace(openAiEndpoint) || string.IsNullOrWhiteSpace(openAiKey))
{
    message = $"You must set the following environment variables to run this workshop:{Environment.NewLine}{Environment.NewLine}" +
        $"- [red]CODEMASH_SK_AZURE_OPENAI_ENDPOINT[/]{Environment.NewLine}" +
        $"- [red]CODEMASH_SK_AZURE_OPENAI_KEY[/]";
}
else
{
    isReady = true;
    message = $"Your machine is configured and ready to go.{Environment.NewLine}{Environment.NewLine}" +
        "Make sure you have an active Azure account you can log into and use to provision AI resources.";
}

AnsiConsole.Write(new Panel(message)
    .Header(" Welcome to the Semantic AI Workshop ")
    .BorderStyle(new Style(isReady ? Color.Green : Color.Orange3))
    .Expand());
