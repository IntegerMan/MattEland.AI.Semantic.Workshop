using Azure;
using Azure.AI.TextAnalytics;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Spectre.Console;
using System.Net;
using System.Text;

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

    public async Task AnalyzeAsync(string documentText)
    {
        //documentText = "Scrooge was at first inclined to be surprised that the Spirit should attach importance to conversations apparently so trivial; but feeling assured that they must have some hidden purpose, he set himself to consider what it was likely to be. They could scarcely be supposed to have any bearing on the death of Jacob, his old partner, for that was Past, and this Ghost’s province was the Future. Nor could he think of any one immediately connected with himself, to whom he could apply them. But nothing doubting that to whomsoever they applied they had some latent moral for his own improvement, he resolved to treasure up every word he heard, and everything he saw; and especially to observe the shadow of himself when it appeared. For he had an expectation that the conduct of his future self would give him the clue he missed, and would render the solution of these riddles easy.";

        AnalyzeActionsOperation? operation = null;
        await AnsiConsole.Status().StartAsync("[Yellow]Performing text analysis...[/]", async ctx =>
        {
            try
            {
                TextAnalyticsActions actions = new()
                {
                    AnalyzeSentimentActions = new List<AnalyzeSentimentAction>() { new() },
                    AbstractiveSummarizeActions = new List<AbstractiveSummarizeAction>() { new() },
                    ExtractKeyPhrasesActions = new List<ExtractKeyPhrasesAction>() { new() },
                    RecognizeEntitiesActions = new List<RecognizeEntitiesAction>() { new() },
                    RecognizeLinkedEntitiesActions = new List<RecognizeLinkedEntitiesAction>() { new() },
                    RecognizePiiEntitiesActions = new List<RecognizePiiEntitiesAction>() { new() },
                    AnalyzeHealthcareEntitiesActions = new List<AnalyzeHealthcareEntitiesAction>() { new() },
                    
                    // Text classification and using custom models is also possible
                };

                // We'll get an ArgumentOutOfRangeException if the text is too short, so only enable this if we've crossed a certain threshold.
                if (documentText.Length > 40)
                {
                    actions.ExtractiveSummarizeActions = new List<ExtractiveSummarizeAction>() { new() };
                }

                string[] documents = [documentText];
                operation = await _client.AnalyzeActionsAsync(WaitUntil.Completed, documents, actions);
            }
            catch (RequestFailedException ex)
            {
                HandleRequestFailedException(ex);
            }
            catch (NotSupportedException ex)
            {
                AnsiConsole.MarkupLine($"[Red]Text analysis failed. The requested actions may not be possible in your Azure region. Error details:[/] {Markup.Escape(ex.Message)}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                AnsiConsole.MarkupLine($"[Red]Text analysis failed. This can happen when the provided text is too short. Error details:[/] {Markup.Escape(ex.Message)}");
            }
        });

        if (operation is not null)
        {
            await foreach (AnalyzeActionsResult result in operation.Value)
            {
                foreach (AbstractiveSummarizeResult abstractResult in result.AbstractiveSummarizeResults.SelectMany(r => r.DocumentsResults))
                {
                    DisplayAbstractiveSummaryResult(abstractResult);
                }
                foreach (ExtractiveSummarizeResult extractResult in result.ExtractiveSummarizeResults.SelectMany(r => r.DocumentsResults))
                {
                    DisplayExtractiveSummaryResult(extractResult);
                }
                foreach (AnalyzeSentimentResult sentimentResult in result.AnalyzeSentimentResults.SelectMany(r => r.DocumentsResults))
                {
                    DisplaySentimentAnalysisResult(sentimentResult.DocumentSentiment);
                }
                foreach (ExtractKeyPhrasesResult keyPhraseResult in result.ExtractKeyPhrasesResults.SelectMany(r => r.DocumentsResults))
                {
                    DisplayKeyPhrasesResult(keyPhraseResult);
                }
                foreach (RecognizeEntitiesResult entityResult in result.RecognizeEntitiesResults.SelectMany(r => r.DocumentsResults))
                {
                    DisplayEntitiesResult(entityResult);
                }
                foreach (RecognizeLinkedEntitiesResult linkedEntityResult in result.RecognizeLinkedEntitiesResults.SelectMany(r => r.DocumentsResults))
                {
                    DisplayLinkedEntitiesResult(linkedEntityResult);
                }
                foreach (RecognizePiiEntitiesResult piiEntityResult in result.RecognizePiiEntitiesResults.SelectMany(r => r.DocumentsResults))
                {
                    DisplayPiiEntitiesResult(piiEntityResult);
                }
                foreach (AnalyzeHealthcareEntitiesResult healthcareEntityResult in result.AnalyzeHealthcareEntitiesResults.SelectMany(r => r.DocumentsResults))
                {
                    DisplayHealthcareEntitiesResult(healthcareEntityResult);
                }
            }
        }
    }

    private static void DisplayAbstractiveSummaryResult(AbstractiveSummarizeResult result)
    {
        if (result.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]AbstractiveSummarize failed with error:[/] {Markup.Escape(result.Error.Message)}");
            return;
        }

        StringBuilder sb = new();
        foreach (AbstractiveSummary summary in result.Summaries)
        {
            sb.AppendLine(summary.Text);
        }

        DisplayHelpers.DisplayBorderedMessage("Abstractive Summary", sb.ToString());
    }

    private static void DisplayExtractiveSummaryResult(ExtractiveSummarizeResult item)
    {
        if (item.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]ExtractiveSummarize failed with error:[/] {Markup.Escape(item.Error.Message)}");
            return;
        }

        StringBuilder sb = new();
        foreach (ExtractiveSummarySentence sentence in item.Sentences)
        {
            sb.AppendLine();
            sb.AppendLine($"[Yellow]>[/] [Italic]{Markup.Escape(sentence.Text)}[/]");
        }

        DisplayHelpers.DisplayBorderedMessage("Extractive Summary", sb.ToString());
    }

    private static void DisplaySentimentAnalysisResult(DocumentSentiment result)
    {
        // Identify the core values we care about
        TextSentiment likelySentiment = result.Sentiment;
        double positiveConfidence = result.ConfidenceScores.Positive;
        double neutralConfidence = result.ConfidenceScores.Neutral;
        double negativeConfidence = result.ConfidenceScores.Negative;

        // Build a bar chart to display the result
        BarChart chart = new BarChart()
            .Label("Sentiment Confidence Percentages")
            .LeftAlignLabel()
            .WithMaxValue(100)
            // Confidence %s range from 0 to 1.0 with 1.0 being 100%. We multiply by 100 to get a percentage for the bar chart.
            .AddItem("[Green]Positive[/]", positiveConfidence * 100, GetSentimentColor(TextSentiment.Positive))
            .AddItem("[Yellow]Neutral[/]", neutralConfidence * 100, GetSentimentColor(TextSentiment.Neutral))
            .AddItem("[Red]Negative[/]", negativeConfidence * 100, GetSentimentColor(TextSentiment.Negative));

        DisplayHelpers.DisplayBorderedMessage($"Sentiment Analysis: {likelySentiment}", chart, GetSentimentColor(result.Sentiment));
    }

    private static Color GetSentimentColor(TextSentiment sentiment) => sentiment switch
    {
        TextSentiment.Positive => Color.Green,
        TextSentiment.Neutral => Color.Yellow,
        TextSentiment.Negative => Color.Red,
        TextSentiment.Mixed => Color.Orange1,
        _ => Color.White // Shouldn't happen
    };

    private static void DisplayKeyPhrasesResult(ExtractKeyPhrasesResult keyPhraseResult)
    {
        if (keyPhraseResult.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]KeyPhraseExtraction failed with error:[/] {Markup.Escape(keyPhraseResult.Error.Message)}");
            return;
        }

        if (keyPhraseResult.KeyPhrases.Count == 0)
        {
            AnsiConsole.MarkupLine("[Yellow]No key phrases found.[/]");
            return;
        }
        DisplayHelpers.DisplayBorderedMessage("Key Phrases", string.Join(", ", keyPhraseResult.KeyPhrases.Select(p => $"[Yellow]{Markup.Escape(p)}[/]")));
    }

    private static void DisplayEntitiesResult(RecognizeEntitiesResult entityResult)
    {
        if (entityResult.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]RecognizeEntities failed with error:[/] {Markup.Escape(entityResult.Error.Message)}");
            return;
        }

        if (entityResult.Entities.Count == 0)
        {
            AnsiConsole.MarkupLine("[Yellow]No entities found.[/]");
            return;
        }
        DisplayHelpers.DisplayBorderedMessage("Entities", string.Join(", ", entityResult.Entities.Select(e => $"[Yellow]{Markup.Escape(e.Text)}[/]")));
    }

    private static void DisplayLinkedEntitiesResult(RecognizeLinkedEntitiesResult entityResult)
    {
        if (entityResult.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]RecognizeLinkedEntities failed with error:[/] {Markup.Escape(entityResult.Error.Message)}");
            return;
        }

        if (entityResult.Entities.Count == 0)
        {
            AnsiConsole.MarkupLine("[Yellow]No linked entities found.[/]");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine();
        foreach (var entity in entityResult.Entities)
        {
            sb.AppendLine($"- [Yellow]{entity.Name}:[/] [Blue]{entity.Url}[/]");
        }

        DisplayHelpers.DisplayBorderedMessage("Linked Entities", sb.ToString());
    }

    private static void DisplayPiiEntitiesResult(RecognizePiiEntitiesResult piiEntityResult)
    {
        if (piiEntityResult.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]PII failed with error:[/] {Markup.Escape(piiEntityResult.Error.Message)}");
            return;
        }

        if (piiEntityResult.Entities.Count == 0)
        {
            AnsiConsole.MarkupLine("[Yellow]No PII entities found.[/]");
            return;
        }

        StringBuilder sb = new();
        foreach (PiiEntity entity in piiEntityResult.Entities.DistinctBy(e => e.Text))
        {
            string categoryName = entity.Category.ToString();
            if (entity.SubCategory is not null)
            {
                categoryName += $"/{entity.SubCategory}";
            }

            sb.AppendLine($"- [Yellow]{entity.Text}:[/] in category [SteelBlue]{categoryName}[/] with {entity.ConfidenceScore:P} confidence (offset {entity.Offset})");
        }

        DisplayHelpers.DisplayBorderedMessage("Personally Identifiable Information (PII)", sb.ToString());
    }

    private static void DisplayHealthcareEntitiesResult(AnalyzeHealthcareEntitiesResult healthcareEntityResult)
    {
        if (healthcareEntityResult.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]HealthCare Entities failed with error:[/] {Markup.Escape(healthcareEntityResult.Error.Message)}");
            return;
        }

        if (healthcareEntityResult.Entities.Count == 0)
        {
            AnsiConsole.MarkupLine("[Yellow]No healthcare entities found.[/]");
            return;
        }

        StringBuilder sb = new();
        foreach (HealthcareEntity entity in healthcareEntityResult.Entities.DistinctBy(e => e.Text))
        {
            sb.AppendLine($"- [Yellow]{entity.Text}:[/] in category [SteelBlue]{entity.Category}[/] with {entity.ConfidenceScore:P} confidence (offset {entity.Offset})");
        }

        DisplayHelpers.DisplayBorderedMessage("Healthcare Entities", sb.ToString());
    }

    private static void HandleRequestFailedException(RequestFailedException ex)
    {
        switch (ex.Status)
        {
            case (int)HttpStatusCode.Unauthorized:
                AnsiConsole.MarkupLine("[Red]Request failed due to authentication failure. Check your AI services key and endpoint.[/]");
                break;
            case (int)HttpStatusCode.TooManyRequests:
                AnsiConsole.MarkupLine("[Red]Request failed due to throttling. Please try again later.[/]");
                break;
            default:
                AnsiConsole.MarkupLine($"[Red]Request failed with status code {ex.Status} and message:[/] {Markup.Escape(ex.Message)}");
                break;
        }
    }
}
