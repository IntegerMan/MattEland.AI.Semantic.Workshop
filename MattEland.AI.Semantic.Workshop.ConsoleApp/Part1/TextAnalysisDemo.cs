using Azure;
using Azure.AI.TextAnalytics;
using Spectre.Console;
using System.Net;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;

public class TextAnalysisDemo
{
    private readonly TextAnalyticsClient _client;

    public TextAnalysisDemo(string endpoint, string key)
    {
        Uri endpointUri = new(endpoint);
        AzureKeyCredential credential = new(key);
        _client = new TextAnalyticsClient(endpointUri, credential);
    }

    public async Task AnalyzeSentimentAsync()
    {
        // Analyze some text
        string text = AnsiConsole.Ask<string>("[Yellow]Enter text for sentiment analysis:[/]");
        // TODO: Get the text from the user
        AnsiConsole.MarkupLine($"[Yellow]Analyzing:[/] {text}");
        AnsiConsole.WriteLine();

        // Analyze the text
        DocumentSentiment? result = null;
        await AnsiConsole.Status().StartAsync("[Yellow]Performing sentiment analysis...[/]", async ctx =>
        {
            try
            {
                result = await _client.AnalyzeSentimentAsync(text);
            }
            catch (RequestFailedException ex)
            {
                switch (ex.Status)
                {
                    case (int)HttpStatusCode.Unauthorized:
                        AnsiConsole.MarkupLine("[Red]Sentiment analysis failed due to authentication failure. Check your AI services key and endpoint.[/]");
                        return;
                    case (int)HttpStatusCode.TooManyRequests:
                        AnsiConsole.MarkupLine("[Red]Sentiment analysis failed due to throttling. Please try again later.[/]");
                        return;
                    default:
                        AnsiConsole.MarkupLine($"[Red]Sentiment analysis failed with status code {ex.Status} and message:[/] {Markup.Escape(ex.Message)}");
                        return;
                }
                
            }
        });

        // This can be null if an exception occurred
        if (result is null)
        {
            return;
        }

        // This produces an overall sentiment indicating the most likely sentiment
        AnsiConsole.MarkupLine($"[Yellow]Sentiment:[/] {result.Sentiment}");

        // Display the results as a bar chart
        AnsiConsole.Write(new BarChart()
            .Label("Sentiment Analysis Confidence %")
            .CenterLabel()
            .WithMaxValue(100)
            // Confidence %s range from 0 to 1.0 with 1.0 being 100%. We multiply by 100 to get a percentage for the bar chart.
            .AddItem("[Green]Positive[/]", result.ConfidenceScores.Positive * 100, Color.Green)
            .AddItem("[Yellow]Neutral[/]", result.ConfidenceScores.Neutral * 100, Color.Yellow)
            .AddItem("[Red]Negative[/]", result.ConfidenceScores.Negative * 100, Color.Red));
    }
}
