using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Application.Interfaces;

public interface IReadContentSearchDocuments
{
    Task<Article> GetArticle(string articleId);
    Task<bool> ArticleExists(string articleId);
}
