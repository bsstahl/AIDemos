using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Documents.Interfaces;

public interface IReadContentSearchDocuments
{
    Task<Article> GetArticle(string articleId);
    Task<bool> ArticleExists(string articleId);
}
