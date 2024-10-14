using Beary.Entities;
using Beary.Interfaces;
using Beary.Application.Extensions;

namespace Beary.Application;

public class OneShot
{
    public OneShot(IGetEmbeddings embeddingClient, ICreateChatCompletions chatClient)
    {
        ArgumentNullException.ThrowIfNull(embeddingClient, nameof(embeddingClient));
        ArgumentNullException.ThrowIfNull(chatClient, nameof(chatClient));

        _embeddingClient = embeddingClient;
        _chatClient = chatClient;
    }

    private readonly ICreateChatCompletions _chatClient;
    private readonly IGetEmbeddings _embeddingClient;

    public async Task<IEnumerable<ChatContent>> GetChatResponse(string userQuery, IEnumerable<string> documents)
    {
        var chatContents = new List<ChatContent>();

        chatContents.Add(ChatContent.From(Constants.Prompts.SystemPrompt, ChatRole.System));
        chatContents.AddRange(documents.AsChatContents(ChatRole.Context));
        chatContents.Add(ChatContent.From(userQuery, ChatRole.User));

        chatContents.Add(await chatContents.GetChatCompletions(_chatClient));
        return chatContents;
    }

}
