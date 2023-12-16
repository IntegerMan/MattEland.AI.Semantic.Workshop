using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public enum Part2MenuOptions
{
    [Description("Text Completion")]
    TextCompletion,
    [Description("Chat Completion")]
    ChatCompletion,
    [Description("Text Embedding")]
    TextEmbedding,
    [Description("Go back")]
    Back
}