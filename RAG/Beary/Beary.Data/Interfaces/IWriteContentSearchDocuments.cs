using Beary.ValueTypes;

namespace Beary.Data;

public interface IWriteContentSearchDocuments
{
    Task SaveAsync(Identifier id, ArticleContent content, TokenCount tokenCount);
}