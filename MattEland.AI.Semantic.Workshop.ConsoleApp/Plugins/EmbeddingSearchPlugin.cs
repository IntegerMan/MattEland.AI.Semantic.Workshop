using Azure;
using Azure.AI.OpenAI;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Properties;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Numerics.Tensors;
using System.Text;
using System.Text.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;

public class EmbeddingSearchPlugin
{
    private readonly List<ArticleLinkWithEmbeddings> _articles;
    private readonly OpenAIClient _client;
    private readonly string _deploymentName;

    public EmbeddingSearchPlugin()
    {
        _articles = LoadArticles();
        //_client = client;
        //_deploymentName = deploymentName;
    }

    [KernelFunction, Description("Searches for articles based on a search term")]
    public async Task<string> SearchArticlesAsync(string searchText) // TODO: Need context here!
    {
        try
        {
            EmbeddingsOptions options = new()
            {
                DeploymentName = _deploymentName
            };
            options.Input.Add(searchText);

            Response<Embeddings> response = await _client.GetEmbeddingsAsync(options);

            if (response.Value.Data.Count == 0)
            {
                return "No embeddings found";
            }

            float[] searchEmbeddings = response.Value.Data.First().Embedding.ToArray();

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
