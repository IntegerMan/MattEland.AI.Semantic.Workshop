﻿using Spectre.Console;
using Spectre.Console.Rendering;
using System.ComponentModel;
using System.Reflection;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public static class DisplayHelpers
{

    public static void DisplayBorderedMessage(string header, string message)
    {
        DisplayBorderedMessage(header, message, Color.SteelBlue);
    }

    public static void DisplayBorderedMessage(string header, string message, Color borderColor)
    {
        AnsiConsole.Write(new Panel(message)
                       .Header(header)
                       .BorderStyle(new Style(borderColor))
                       .Expand());
        AnsiConsole.WriteLine();
    }

    public static void DisplayBorderedMessage(string header, IRenderable content)
    {
        DisplayBorderedMessage(header, content, Color.SteelBlue);
    }

    public static void DisplayBorderedMessage(string header, IRenderable content, Color borderColor)
    {
        AnsiConsole.Write(new Panel(content)
                       .Header(header)
                       .BorderStyle(new Style(borderColor))
                       .Expand());
        AnsiConsole.WriteLine();
    }

    public static string ToFriendlyName(this Enum value)
    {
        FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute[]? attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        return attributes?[0].Description ?? value.ToString();
    }
}