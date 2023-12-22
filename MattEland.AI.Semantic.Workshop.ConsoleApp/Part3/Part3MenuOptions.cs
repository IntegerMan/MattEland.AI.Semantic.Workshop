using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part3;

public enum Part3MenuOptions
{
    [Description("Chat with the Kernel")]
    SimpleChat,
    [Description("Chat with prompt template")]
    SimpleChatWithTemplate,
    [Description("Classification Chat Example")]
    Classification,
    [Description("Chaining together Semantic Functions")]
    ChainedFunctions,
    [Description("Go back")]
    Back
}