using Beary.ValueTypes;

namespace Beary.Data.Interfaces;

public interface IWriteContentSearchDocuments
{
    Task SaveAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount);
}