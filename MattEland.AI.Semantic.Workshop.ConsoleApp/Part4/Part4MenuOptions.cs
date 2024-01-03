using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part4;

public enum Part4MenuOptions
{
    [Description("Using Plugins")]
    PluginDemo,
    [Description("Observability with Kernel Events")]
    KernelEvents,
    [Description("Chat using the Handlebars Planner")]
    HandlebarsPlanner,
    [Description("Chat using the Function-Calling Stepwise Planner")]
    FunctionCallingPlanner,
    [Description("Filtering functions from the planner")]
    PlannerFilteringDemo,
    [Description("Lab 3: Build your own plugin(s)")]
    Lab,
    [Description("Go back")]
    Back
}