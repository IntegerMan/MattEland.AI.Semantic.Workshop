using Spectre.Console;
using System.ComponentModel;
using System.Reflection;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public static class DisplayHelpers
{
    public static void DisplayBorderedMessage(string header, string message, Color borderColor)
    {
        AnsiConsole.Write(new Panel(message)
                       .Header(header)
                       .BorderStyle(new Style(borderColor))
                       .Expand());
    }

    public static string ToFriendlyName(this Enum value)
    {
        FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute[]? attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        return attributes?[0].Description ?? value.ToString();
    }
}
