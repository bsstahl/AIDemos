using Beary.Chat.AzureGpt.Extensions;
using Beary.Chat.Entities;
using Beary.Chat.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Beary.Chat.AzureGpt;

public class Client : ICreateChatCompletions
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
}
