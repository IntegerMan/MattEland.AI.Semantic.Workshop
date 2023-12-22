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
    [Description("Chat with a plugin")]
    PluginDemo,
    [Description("Observability with Kernel Events")]
    KernelEvents,
    [Description("Chat using the Handlebars Planner")]
    HandlebarsPlanner,
    [Description("Chat using the Function-Calling Stepwise Planner")]
    FunctionCallingPlanner,
    [Description("Go back")]
    Back
}