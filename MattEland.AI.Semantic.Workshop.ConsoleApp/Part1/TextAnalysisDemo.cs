using Azure;
using Azure.AI.TextAnalytics;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Properties;
using Spectre.Console;
using System.Net;
using System.Text;
using System.Linq;

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

    public async Task AnalyzeAsync()
    {
        AnalyzeActionsOperation? operation = null;
        await AnsiConsole.Status().StartAsync("[Yellow]Performing analysis...[/]", async ctx =>
        {
            try
            {
                TextAnalyticsActions actions = new()
                {
                    DisplayName = "CodeMash Analysis",
                    AnalyzeSentimentActions = new List<AnalyzeSentimentAction>() { new() { ActionName = "AnalyzeSentiment" } },
                    AbstractiveSummarizeActions = new List<AbstractiveSummarizeAction>() { new() { ActionName = "AbstractiveSummarize" } },
                    ExtractiveSummarizeActions = new List<ExtractiveSummarizeAction>() { new() { ActionName = "ExtractiveSummarize" } },
                    ExtractKeyPhrasesActions = new List<ExtractKeyPhrasesAction>() { new() { ActionName = "ExtractKeyPhrases" } },
                    RecognizeEntitiesActions = new List<RecognizeEntitiesAction>() { new() { ActionName = "RecognizeEntities" } },
                    RecognizeLinkedEntitiesActions = new List<RecognizeLinkedEntitiesAction>() { new() { ActionName = "RecognizeLinkedEntities" } },
                    AnalyzeHealthcareEntitiesActions = new List<AnalyzeHealthcareEntitiesAction>() { new() { ActionName = "AnalyzeHealthcareEntities" } },
                    RecognizePiiEntitiesActions = new List<RecognizePiiEntitiesAction>() { new() { ActionName = "RecognizePiiEntities" } },
                    // Text classification actions are also possible, as are using custom models
                };

                IEnumerable<TextDocumentInput> documents = GetDocumentsToAnalyze();

                operation = await _client.AnalyzeActionsAsync(WaitUntil.Completed, documents, actions);
            }
            catch (RequestFailedException ex)
            {
                HandleRequestFailedException(ex);
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
            }
        }
    }

    private static void DisplayEntitiesResult(RecognizeEntitiesResult entityResult)
    {
        if (entityResult.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]Document {entityResult.Id} failed with error:[/] {Markup.Escape(entityResult.Error.Message)}");
            return;
        }

        DisplayHelpers.DisplayBorderedMessage($"Entities for Document {entityResult.Id}", string.Join(", ", entityResult.Entities.Select(e => e.Text)));
    }

    private static void DisplayLinkedEntitiesResult(RecognizeLinkedEntitiesResult entityResult)
    {
        if (entityResult.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]Document {entityResult.Id} failed with error:[/] {Markup.Escape(entityResult.Error.Message)}");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine();
        foreach (var entity in entityResult.Entities)
        {
            sb.AppendLine($"- [Yellow]{entity.Name}:[/] [Blue]{entity.Url}[/]");
        }

        DisplayHelpers.DisplayBorderedMessage($"Linked Entities for Document {entityResult.Id}", sb.ToString());
    }

    private static void DisplayKeyPhrasesResult(ExtractKeyPhrasesResult keyPhraseResult)
    {
        if (keyPhraseResult.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]Document {keyPhraseResult.Id} failed with error:[/] {Markup.Escape(keyPhraseResult.Error.Message)}");
            return;
        }

        DisplayHelpers.DisplayBorderedMessage($"Key Phrases for Document {keyPhraseResult.Id}", string.Join(", ", keyPhraseResult.KeyPhrases));
    }

    public async Task AnalyzeSentimentAsync()
    {
        // Get the text to analyze from the user
        string text = AnsiConsole.Ask<string>("[Yellow]Enter text for sentiment analysis:[/]");
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
                HandleRequestFailedException(ex);
            }
        });

        // This can be null if an exception occurred
        if (result is not null)
        {
            DisplaySentimentAnalysisResult(result);
        }
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

    public async Task AbstractiveSummarizationAsync()
    {
        // Analyze the text
        List<AbstractiveSummarizeResult> summaries = new();
        await AnsiConsole.Status().StartAsync("[Yellow]Performing abstractive summarization...[/]", async ctx =>
        {
            try
            {
                IEnumerable<TextDocumentInput> documents = GetDocumentsToAnalyze();
                AbstractiveSummarizeOperation operation = await _client.AbstractiveSummarizeAsync(WaitUntil.Completed, documents);

                Response<AsyncPageable<AbstractiveSummarizeResultCollection>> result = await operation.WaitForCompletionAsync();

                foreach (AbstractiveSummarizeResultCollection resultCollection in result.Value.ToBlockingEnumerable())
                {
                    summaries.AddRange(resultCollection);
                }
            }
            catch (RequestFailedException ex)
            {
                HandleRequestFailedException(ex);
            }
        });

        foreach (AbstractiveSummarizeResult item in summaries)
        {
            DisplayAbstractiveSummaryResult(item);
        }
    }

    private static void DisplayAbstractiveSummaryResult(AbstractiveSummarizeResult result)
    {
        if (result.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]Document {result.Id} failed with error:[/] {Markup.Escape(result.Error.Message)}");
            return;
        }

        StringBuilder sb = new();
        foreach (AbstractiveSummary summary in result.Summaries)
        {
            sb.AppendLine(summary.Text);
        }

        DisplayHelpers.DisplayBorderedMessage($"Document {result.Id} Abstractive Summary", sb.ToString());
    }

    public async Task ExtractiveSummarizationAsync()
    {
        // Analyze the text
        List<ExtractiveSummarizeResult> summaries = new();
        await AnsiConsole.Status().StartAsync("[Yellow]Performing extractive summarization...[/]", async ctx =>
        {
            try
            {
                IEnumerable<TextDocumentInput> documents = GetDocumentsToAnalyze();
                ExtractiveSummarizeOperation operation = await _client.ExtractiveSummarizeAsync(WaitUntil.Completed, documents);

                Response<AsyncPageable<ExtractiveSummarizeResultCollection>> result = await operation.WaitForCompletionAsync();

                foreach (ExtractiveSummarizeResultCollection resultCollection in result.Value.ToBlockingEnumerable())
                {
                    summaries.AddRange(resultCollection);
                }
            }
            catch (RequestFailedException ex)
            {
                HandleRequestFailedException(ex);
            }
        });

        foreach (ExtractiveSummarizeResult item in summaries)
        {
            DisplayExtractiveSummaryResult(item);
        }
    }

    private static void DisplayExtractiveSummaryResult(ExtractiveSummarizeResult item)
    {
        if (item.HasError)
        {
            AnsiConsole.MarkupLine($"[Red]Document {item.Id} failed with error:[/] {Markup.Escape(item.Error.Message)}");
            return;
        }

        StringBuilder sb = new();

        foreach (ExtractiveSummarySentence sentence in item.Sentences)
        {
            sb.AppendLine();
            sb.AppendLine($"[Yellow]>[/] [Italic]{Markup.Escape(sentence.Text)}[/]");
        }

        DisplayHelpers.DisplayBorderedMessage($"Document {item.Id} Extractive Summary", sb.ToString());
    }

    private static IEnumerable<TextDocumentInput> GetDocumentsToAnalyze()
    {
        yield return new TextDocumentInput("CodeMash Workshop Abstract", Resources.WorkshopAbstract);
        // yield return new TextDocumentInput("SemanticKernel Announcement", Resources.SemanticKernelAnnouncement);
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
