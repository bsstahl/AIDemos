using Beary.Application.Extensions;
using Beary.Application.Interfaces;
using Beary.Entities;
using Beary.Interfaces;

namespace Beary.Application;

public class MultiShot
{
    public MultiShot(IGetEmbeddings embeddingClient, IFindRelevantDocuments searchClient, ICreateChatCompletions chatClient, IReadEmbeddingsSearchDocuments embeddingsReadRepo)
    {
        ArgumentNullException.ThrowIfNull(embeddingClient, nameof(embeddingClient));
        ArgumentNullException.ThrowIfNull(searchClient, nameof(searchClient));
        ArgumentNullException.ThrowIfNull(chatClient, nameof(chatClient));
        ArgumentNullException.ThrowIfNull(embeddingsReadRepo, nameof(embeddingsReadRepo));

        _embeddingClient = embeddingClient;
        _searchClient = searchClient;
        _chatClient = chatClient;
        _embeddingsReadRepo = embeddingsReadRepo;
    }

    private readonly ICreateChatCompletions _chatClient;
    private readonly IFindRelevantDocuments _searchClient;
    private readonly IGetEmbeddings _embeddingClient;
    private readonly IReadEmbeddingsSearchDocuments _embeddingsReadRepo;

    public async Task<IEnumerable<ChatContent>> GetChatResponse(string userQuery, IEnumerable<string> documents, IEnumerable<ChatContent>? previousContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userQuery, nameof(userQuery));

        // Returns the original userQuery if no previousContext is supplied
        var queryText = await _chatClient.Disambiguate(userQuery, previousContext);

        var chatContents = previousContext is null
            ? new List<ChatContent> { ChatContent.From(Constants.Prompts.SystemPrompt, ChatRole.System) }
            : previousContext.Select(c => c).ToList();

        // Add new documents
        documents.ToList().ForEach(d => chatContents.Add(ChatContent.From(d, ChatRole.Context)));

        // Add disambiguated user query
        chatContents.Add(ChatContent.From(queryText, ChatRole.User));

        var response = await _chatClient.CreateChatCompletionsAsync(chatContents);
        chatContents.Add(ChatContent.From(response.Value, ChatRole.Agent));

        return chatContents;
    }


}
