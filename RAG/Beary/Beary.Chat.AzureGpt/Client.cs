using Beary.Chat.AzureGpt.Extensions;
using Beary.Chat.Entities;
using Beary.Chat.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Beary.Chat.AzureGpt;

public class Client : ICreateChatCompletions, IDisambiguateQueries
{
    private readonly OpenAI.Chat.ChatClient _chatClient;

    public Client(IConfiguration config)
    {
        var token = config["AzureGpt:ApiToken"] ?? string.Empty;
        var url = config["AzureGpt:ApiEndpoint"] ?? string.Empty;
        var deploymentName = config["AzureGpt:DeploymentName"] ?? string.Empty;

        var cred = new System.ClientModel.ApiKeyCredential(token);
        var uri = new Uri(url);
        var options = new Azure.AI.OpenAI.AzureOpenAIClientOptions();

        _chatClient = new Azure.AI.OpenAI.AzureOpenAIClient(uri, cred, options).GetChatClient(deploymentName);
    }

    public async Task<ChatContent> CreateChatCompletionsAsync(IEnumerable<ChatContent> chatContext)
    {
        var messages = chatContext.AsChatMessages();

        var response = await _chatClient
            .CompleteChatAsync(messages)
            .ConfigureAwait(false);

        var content = string.Join("\r\n", response.Value.Content.Select(c => c.Text));
        return ChatContent.From(content, ChatRole.Agent);
    }

    public async Task<string> Disambiguate(string text, IEnumerable<ChatContent>? chatContents)
    {
        string systemPrompt = "You are an expert at interpreting and clarify ambiguous language in user input. Your primary objective is to fully qualify all ambiguous words and phrases based on the context of the conversation, ensuring clear and precise communication.";
        string disambiguationRequest = "Restate the user's most recent request, and ONLY their most recent request, fully disambiguated. Respond only with the restated user request in the voice of the user.";

        ArgumentNullException.ThrowIfNullOrWhiteSpace(text, nameof(text));

        var contents = new List<ChatContent>()
        {
            ChatContent.From(systemPrompt, ChatRole.System)
        };

        contents.AddRange(chatContents?.Where(c => c.Role != ChatRole.System) ?? []);
        contents.Add(ChatContent.From(disambiguationRequest, ChatRole.User));
        contents.Add(ChatContent.From(text, ChatRole.User));

        var response = await CreateChatCompletionsAsync(contents).ConfigureAwait(false);

        return response.Value;
    }
}
