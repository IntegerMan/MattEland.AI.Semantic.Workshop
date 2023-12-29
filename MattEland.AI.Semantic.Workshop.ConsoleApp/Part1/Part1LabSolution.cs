using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.AI.Vision.Common;
using Spectre.Console;
using System.Text;
using MattEland.AI.Semantic.Workshop.ConsoleApp.Helpers;
using Microsoft.CognitiveServices.Speech;
using Azure.AI.TextAnalytics;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;

public class Part1LabSolution
{
    private readonly AzureAISettings _settings;

    public Part1LabSolution(AzureAISettings settings)
    {
        _settings = settings;
    }

    public async Task RunAsync()
    {
        // In this lab, you'll use Azure AI Services to listen to words spoken into the microphone, analyze them with Azure AI Language, and then speak a summary back to the user.
        // It's up to you what aspects of Azure AI Language you use, but key phrase extraction, abstractive summarization, and sentiment analysis are good ideas to consider.        
        // You should need to create a speech client as well as a language client in order to accomplish this task. Use values from _settings to accomplish this.

        // Recognize speech
        SpeechConfig speechConfig = SpeechConfig.FromSubscription(_settings.Key, _settings.Region);
        speechConfig.SpeechSynthesisVoiceName = _settings.VoiceName;
        SpeechRecognizer recognizer = new(speechConfig);

        AnsiConsole.MarkupLine("[Yellow]Speak your sentence to be analyzed[/]");

        SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync();

        string text = result.Text;
        AnsiConsole.MarkupLine($"[Yellow]Recognized:[/] {Markup.Escape(text)}");

        // Extract key phrases
        Uri endpoint = new(_settings.Endpoint);
        TextAnalyticsClient textClient = new(endpoint, new AzureKeyCredential(_settings.Key));

        Response<KeyPhraseCollection> response = await textClient.ExtractKeyPhrasesAsync(text);
        KeyPhraseCollection keyPhrases = response.Value;

        // Speak the key phrases
        string voicePrompt = $"The key phrases in your sentence are: {string.Join(", ", keyPhrases)}";
        AnsiConsole.MarkupLine($"[Yellow]Speaking:[/] {Markup.Escape(voicePrompt)}");

        SpeechSynthesizer synthesizer = new(speechConfig);
        await synthesizer.SpeakTextAsync(voicePrompt);
    }
}