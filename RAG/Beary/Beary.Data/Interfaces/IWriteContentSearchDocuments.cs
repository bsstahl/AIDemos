using Beary.ValueTypes;

namespace Beary.Data.Interfaces;

public interface IWriteContentSearchDocuments
{
    Task SaveAsync(Identifier id, ArticleContent content, TokenCount tokenCount);
}