using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Data.Db;

public interface IBearyDataStore
{
    Task SaveArticleAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount);
    Task SaveEmbeddingAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding);
    Task<Article> GetArticleAsync(string articleId);
    Task<bool> ArticleExistsAsync(string articleId);
    Task<long> GetEmbeddingDocumentCountAsync();
    Task<IEnumerable<SearchResult>> GetNearestNeighborsAsync(IEnumerable<float> queryVector, int neighborCount);
    Task<IEnumerable<SearchResult>> GetAllEmbeddingsAsync();
}
