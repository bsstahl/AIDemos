using Beary.ValueTypes;

namespace Beary.Data;

public interface IWriteEmbeddingsSearchDocuments
{
    Task SaveAsync(Identifier id, ArticleContent contentChunk, Location fullArticleLocation, Vector? embedding);
}