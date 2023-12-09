using Spectre.Console;
using System.Text;

// Using UTF8 allows more capabilities for Spectre.Console.
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// TODO: Verify the presence of Environment Variables

string welcomeMarkdown = $"Your machine is configured and ready to go.{Environment.NewLine}{Environment.NewLine}" +
    "Make sure you have an active Azure account you can log into and use to provision AI resources.";

AnsiConsole.Write(new Panel(welcomeMarkdown)
    .Header(" Welcome to the Semantic AI Workshop ")
    .BorderStyle(new Style(Color.Green))
    .Expand());
