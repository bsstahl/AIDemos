using Beary.Data.Db;
using Beary.Documents.Interfaces;
using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Data.AzureAISearch;

internal sealed class AzureAISearchDataStore : IBearyDataStore
{
    private readonly Content.ReadRepository _contentReader;
    private readonly Content.WriteRepository _contentWriter;
    private readonly Embeddings.ReadRepository _embeddingsReader;
    private readonly Embeddings.WriteRepository _embeddingsWriter;

    public AzureAISearchDataStore(string searchServiceName, string apiKey)
    {
        _contentReader = new Content.ReadRepository(searchServiceName, apiKey);
        _contentWriter = new Content.WriteRepository(searchServiceName, apiKey);
        _embeddingsReader = new Embeddings.ReadRepository(searchServiceName, apiKey);
        _embeddingsWriter = new Embeddings.WriteRepository(searchServiceName, apiKey);
    }

    public Task SaveArticleAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount)
        => _contentWriter.SaveAsync(id, title, content, tokenCount);

    public Task SaveEmbeddingAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding)
        => _embeddingsWriter.SaveAsync(id, elementIndex, contentChunk, fullArticleId, embedding);

    public Task<Article> GetArticleAsync(string articleId)
        => _contentReader.GetArticle(articleId);

    public Task<bool> ArticleExistsAsync(string articleId)
        => _contentReader.ArticleExists(articleId);

    public Task<long> GetEmbeddingDocumentCountAsync()
        => _embeddingsReader.GetDocumentCount();

    public Task<IEnumerable<SearchResult>> GetNearestNeighborsAsync(IEnumerable<float> queryVector, int neighborCount)
        => _embeddingsReader.GetNearestNeighbors(queryVector, neighborCount);

    public Task<IEnumerable<SearchResult>> GetAllEmbeddingsAsync()
        => _embeddingsReader.GetAllEmbeddings();
}
