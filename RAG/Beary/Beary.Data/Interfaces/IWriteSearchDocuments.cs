using Beary.ValueTypes;

namespace Beary.Data;

public interface IWriteSearchDocuments
{
    Task SaveAsync(Identifier id, ArticleContent contentChunk, Location fullArticleLocation, Vector? embedding);
}