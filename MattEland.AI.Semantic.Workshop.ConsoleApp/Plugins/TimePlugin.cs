using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;

public class TimePlugin
{
    [KernelFunction]
    [Description("Retrieves the current time in the local time zone.")]
    public string GetCurrentTime() => DateTime.Now.ToString("R");
}
