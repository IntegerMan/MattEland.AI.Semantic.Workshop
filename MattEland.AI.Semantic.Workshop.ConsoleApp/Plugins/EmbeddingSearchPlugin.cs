using Azure;
using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Properties;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using System.ComponentModel;
using System.Numerics.Tensors;
using System.Text;
using System.Text.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class EmbeddingSearchPlugin
{
    private readonly ITextEmbeddingGenerationService _embeddingService;
    private readonly List<ArticleLinkWithEmbeddings> _articles;

    public EmbeddingSearchPlugin(ITextEmbeddingGenerationService embeddingService)
    {
        _embeddingService = embeddingService;
        _articles = LoadArticles();
    }

    [KernelFunction, Description("Searches for articles based on a search term")]
    public async Task<string> SearchArticlesAsync(string searchText)
    {
        try
        {
            IList<ReadOnlyMemory<float>> embeddings = await _embeddingService.GenerateEmbeddingsAsync([searchText]);

            if (embeddings.Count == 0 || embeddings.First().Length == 0)
            {
                return "No embeddings found";
            }

            float[] searchEmbeddings = embeddings.First().ToArray();

            IEnumerable<ArticleLinkWithEmbeddings> articles = ScoreEmbeddingSimilarity(searchEmbeddings).OrderByDescending(a => a.Score).Take(5);
            if (articles.Any())
            {
                StringBuilder sb = new($"I found some results related to {searchText}:");
                foreach (var article in articles)
                {
                    sb.AppendLine($"{article.Name} at {article.Url}");
                }
                return sb.ToString();
            }
            else
            {
                return "No articles found";
            }
        }
        catch (RequestFailedException ex)
        {
            return $"Could not search for text: {ex.Message}";
        }
    }

    private IEnumerable<ArticleLinkWithEmbeddings> ScoreEmbeddingSimilarity(float[] sourceEmbeddings)
    {
        foreach (ArticleLinkWithEmbeddings article in _articles)
        {
            double score = TensorPrimitives.CosineSimilarity(article.Embeddings, sourceEmbeddings);
            article.Score = score;

            yield return article;
        }
    }

    private static List<ArticleLinkWithEmbeddings> LoadArticles() 
        => JsonSerializer.Deserialize<List<ArticleLinkWithEmbeddings>>(Resources.SearchableEmbeddings)!;
}

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
