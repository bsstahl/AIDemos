using Beary.ValueTypes;

namespace Beary.Documents.Interfaces;

public interface IWriteEmbeddingsSearchDocuments
{
    Task SaveAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding);
}