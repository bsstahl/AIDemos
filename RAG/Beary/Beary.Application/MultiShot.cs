using Beary.Application.Extensions;
using Beary.Application.Interfaces;
using Beary.Entities;
using Beary.Interfaces;

namespace Beary.Application;

public class MultiShot
{
    public MultiShot(IGetEmbeddings embeddingClient, IReadEmbeddingsSearchDocuments embeddingsReadRepo, IReadContentSearchDocuments contentReadRepo, ICreateChatCompletions chatClient)
    {
        ArgumentNullException.ThrowIfNull(embeddingClient, nameof(embeddingClient));
        ArgumentNullException.ThrowIfNull(embeddingsReadRepo, nameof(embeddingsReadRepo));
        ArgumentNullException.ThrowIfNull(contentReadRepo, nameof(contentReadRepo));
        ArgumentNullException.ThrowIfNull(chatClient, nameof(chatClient));

        _embeddingClient = embeddingClient;
        _embeddingsReadRepo = embeddingsReadRepo;
        _contentReadRepo = contentReadRepo;
        _chatClient = chatClient;
    }

    private readonly ICreateChatCompletions _chatClient;
    private readonly IGetEmbeddings _embeddingClient;
    private readonly IReadEmbeddingsSearchDocuments _embeddingsReadRepo;
    private readonly IReadContentSearchDocuments _contentReadRepo;

    public async Task<IEnumerable<ChatContent>> GetChatResponse(string userQuery, IEnumerable<ChatContent>? previousContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userQuery, nameof(userQuery));

        // Returns the original userQuery if no previousContext is supplied
        var queryText = await _chatClient.Disambiguate(userQuery, previousContext);

        var chatContents = previousContext is null
            ? new List<ChatContent> { ChatContent.From(Constants.Prompts.SystemPrompt, ChatRole.System) }
            : previousContext.Select(c => c).ToList();

        // Add new documents
        var embedding = await _embeddingClient.GetEmbeddings(new string[] { queryText }, "user");
        var queryEmbedding = embedding.FirstOrDefault()?.Embedding?.Value ?? throw new InvalidOperationException("Unable to retrieve embedding");
        var documentChunks = await _embeddingsReadRepo.GetNearestNeighbors(queryEmbedding, Constants.Search.MaxNeighbors);
        var documentIds = documentChunks.Select(c => c.ItemId).Distinct().ToList();

        var documentTasks = documentIds
            .Select(id => _contentReadRepo.GetArticle(id))
            .ToArray();
        Task.WaitAll(documentTasks);

        var documents = documentTasks
            .Where(t => t.Result?.Content?.Value is not null)
            .Select(t => t.Result?.Content?.Value);
        documents?.ToList().ForEach(d => chatContents.Add(ChatContent.From(d!, ChatRole.Context)));

        // Add disambiguated user query
        chatContents.Add(ChatContent.From(queryText, ChatRole.User));

        var response = await _chatClient.CreateChatCompletionsAsync(chatContents);
        chatContents.Add(ChatContent.From(response.Value, ChatRole.Agent));

        return chatContents;
    }


}
