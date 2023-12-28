using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;

public enum Part1MenuOptions
{
    [Description("Analyze Text")]
    AnalyzeText,
    [Description("Analyze Images")]
    AnalyzeImage,
    [Description("Remove Background")]
    BackgroundRemoval,
    [Description("Text to Speech")]
    TextToSpeech,
    [Description("Speech to Text")]
    SpeechToText,
    [Description("Lab 1: Listen, Summarize, and Speak")]
    Lab,
    [Description("Go back")]
    Back
}