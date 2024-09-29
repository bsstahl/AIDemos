using Beary.ValueTypes;

namespace Beary.Documents.Interfaces;

public interface IWriteContentSearchDocuments
{
    Task SaveAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount);
}