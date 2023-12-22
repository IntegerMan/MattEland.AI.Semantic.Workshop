using Microsoft.CognitiveServices.Speech;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;

public class SpeechDemo
{
    private readonly SpeechConfig _config;

    public SpeechDemo(string region, string key, string voiceName = "en-US-AriaNeural")
    {
        _config = SpeechConfig.FromSubscription(key, region);
        _config.SpeechSynthesisVoiceName = voiceName;
    }

    public async Task SpeakAsync(string text)
    {
        using SpeechSynthesizer synthesizer = new(_config);
        await synthesizer.SpeakTextAsync(text);
    }

    public async Task<string?> RecognizeSpeechAsync()
    {
        using SpeechRecognizer recognizer = new(_config);

        AnsiConsole.MarkupLine("[Yellow]Listening for speech...[/]");
        SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            AnsiConsole.MarkupLine($"[Yellow]Recognized:[/] {result.Text}");
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            AnsiConsole.MarkupLine($"[Red]NOMATCH: Speech could not be recognized. Your mic may be having issues.[/]");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            AnsiConsole.MarkupLine($"[Red]CANCELED: Reason={cancellation.Reason}[/]");

            if (cancellation.Reason == CancellationReason.Error)
            {
                AnsiConsole.MarkupLine($"[Red]CANCELED: ErrorCode={cancellation.ErrorCode}[/]");
                AnsiConsole.MarkupLine($"[Red]CANCELED: ErrorDetails={cancellation.ErrorDetails}[/]");
            }
        }


        return result.Text;
    }
}