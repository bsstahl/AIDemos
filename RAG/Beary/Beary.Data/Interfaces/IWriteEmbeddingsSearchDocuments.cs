using Beary.ValueTypes;

namespace Beary.Data.Interfaces;

public interface IWriteEmbeddingsSearchDocuments
{
    Task SaveAsync(Identifier id, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding);
}