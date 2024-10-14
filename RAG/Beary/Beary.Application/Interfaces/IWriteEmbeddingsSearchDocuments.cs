using Beary.ValueTypes;

namespace Beary.Application.Interfaces;

public interface IWriteEmbeddingsSearchDocuments
{
    Task SaveAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding);
}