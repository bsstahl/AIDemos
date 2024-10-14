using Beary.ValueTypes;

namespace Beary.Application.Interfaces;

public interface IWriteContentSearchDocuments
{
    Task SaveAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount);
}