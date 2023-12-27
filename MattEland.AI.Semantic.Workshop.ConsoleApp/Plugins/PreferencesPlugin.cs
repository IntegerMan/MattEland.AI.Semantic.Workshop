using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins;

public class PreferencesPlugin
{
    [KernelFunction, Description("Returns the user's clothing preferences for different combinations of temperature and weather conditions")]
    public string ClothingPreferences()
    {
        return "If it is cold, the user prefers to wear a heavy coat and either a wool fedora when it's sunny, wool ivy hat when it's not, or a fur trapper hat for extreme temperatures." +
            "For moderate temperatures, the user prefers to wear a light jacket and either a leather safari hat when it's sunny, or a leather ivy cap when it's not." +
            "When it's warm, the user prefers no jacket and a vented hiking hat." +
            "On extremely hot days without rain, the user prefers a soaker hat." +
            "When it rains, the user will wear whatever jacket is appropriate for the temperature and a rain hat.";
    }
}
