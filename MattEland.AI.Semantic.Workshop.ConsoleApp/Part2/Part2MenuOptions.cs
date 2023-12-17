using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public enum Part2MenuOptions
{
    [Description("Text Completion")]
    TextCompletion,
    [Description("Chat Completion")]
    ChatCompletion,
    [Description("Image Generation (DALL-E)")]
    ImageCompletion,
    [Description("Generate Text Embeddings")]
    GenerateTextEmbedding,
    [Description("Search Text Embeddings")]
    SearchEmbedding,
    [Description("Go back")]
    Back
}