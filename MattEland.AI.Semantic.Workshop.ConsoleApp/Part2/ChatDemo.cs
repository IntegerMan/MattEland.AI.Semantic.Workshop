using Azure;
using Azure.AI.OpenAI;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Part2;

public class ChatDemo
{
    private readonly ChatCompletionsOptions _history;
    private readonly OpenAIClient _client;

    public ChatDemo(AppSettings settings, string systemPrompt)
    {
        bool useAzureOpenAi = settings.AzureOpenAI.IsConfigured;
        string deploymentName = useAzureOpenAi 
            ? settings.AzureOpenAI.ChatDeploymentName 
            : settings.OpenAI.ChatModel;

        _history = new ChatCompletionsOptions()
        {
            MaxTokens = 4000,
            DeploymentName = deploymentName,
            Temperature = 1f, // ranges from 0 to 2
            // You can connect to Azure AI Search to perform Retrieval Augmentation Generation (RAG) by providing Azure Extensions
        };
        _history.Messages.Add(new ChatRequestSystemMessage(systemPrompt));

        if (useAzureOpenAi)
        {
            Uri uri = new(settings.AzureOpenAI.Endpoint);
            AzureKeyCredential key = new(settings.AzureOpenAI.Key);
            _client = new OpenAIClient(uri, key);
        }
        else
        {
            _client = new OpenAIClient(settings.OpenAI.Key);
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
                AnsiConsole.MarkupLine($"[Red]Deployment not found. Please check your settings.[/]");
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
