using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public enum Part3MenuOptions
{
    [Description("Chat with the Kernel")]
    SimpleChat,
    [Description("Chat with a Semantic Function")]
    SemanticFunction,
    [Description("Chaining together Semantic Functions")]
    ChainedFunctions,
    [Description("Go back")]
    Back
}