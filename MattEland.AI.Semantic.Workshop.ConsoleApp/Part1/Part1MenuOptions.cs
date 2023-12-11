using System.ComponentModel;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public enum Part1MenuOptions
{
    [Description("Analyze Text")]
    AnalyzeText,
    [Description("Analyze Images")]
    AnalyzeImage,
    [Description("Text to Speech")]
    TextToSpeech,
    [Description("Speech to Text")]
    SpeechToText,
    [Description("Go back")]
    Back
}