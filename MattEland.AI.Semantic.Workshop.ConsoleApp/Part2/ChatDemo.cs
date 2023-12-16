using Azure;
using Azure.AI.OpenAI;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class ChatDemo
{
    private readonly Part2Settings _settings;
    private readonly ChatCompletionsOptions _history;
    private readonly OpenAIClient _client;

    public ChatDemo(Part2Settings settings, string systemPrompt)
    {
        _settings = settings;
        _history = new ChatCompletionsOptions()
        {
            MaxTokens = 4000,
            DeploymentName = settings.ChatDeployment,
            Temperature = 1f, // ranges from 0 to 2
            // You can connect to Azure AI Search to perform Retrieval Augmentation Generation (RAG) by providing Azure Extensions
        };
        _history.Messages.Add(new ChatRequestSystemMessage(systemPrompt));

        if (string.IsNullOrEmpty(settings.OpenAiEndpoint))
        {
            _client = new OpenAIClient(settings.OpenAiKey);
        }
        else
        {
            Uri uri = new(settings.OpenAiEndpoint);
            AzureKeyCredential key = new(settings.OpenAiKey);
            _client = new OpenAIClient(uri, key);
        }

    }

    public async Task<string?> GetChatCompletionAsync(string userText)
    {
        try
        {
            // Add the user's text to the history
            _history.Messages.Add(new ChatRequestUserMessage(userText));

            // Get the response from OpenAI
            Response<ChatCompletions> result = await _client.GetChatCompletionsAsync(_history);

            // Add the response to the history
            ChatResponseMessage responseMessage = result.Value.Choices.First().Message;
            _history.Messages.Add(new ChatRequestAssistantMessage(responseMessage.Content));

            return responseMessage.Content;
        }
        catch (RequestFailedException ex)
        {
            if (ex.ErrorCode == "DeploymentNotFound")
            {
                AnsiConsole.MarkupLine($"[Red]Deployment {_settings.ChatDeployment} not found. Please check your settings.[/]");
            }
            else if (ex.ErrorCode == "MaxTokensError")
            {
                AnsiConsole.MarkupLine($"[Red]Max tokens exceeded. Please try a shorter prompt.[/]");
            }
            else if (ex.ErrorCode == "CompletionNotFoundError")
            {
                AnsiConsole.MarkupLine($"[Red]Completion not found. Please try a different prompt.[/]");
            }
            else if (ex.ErrorCode == "OperationNotSupported")
            {
                AnsiConsole.MarkupLine($"[Red]Operation not supported. You may need to deploy a different model. gpt-35-turbo is known to work.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[Red]Error: {ex.Message} ({ex.GetType().Name})[/]");
                AnsiConsole.MarkupLine($"[Red]Status Code: {ex.Status}[/]");
                AnsiConsole.MarkupLine($"[Red]Error Code: {ex.ErrorCode}[/]");
            }
        }

        return null;
    }
}
