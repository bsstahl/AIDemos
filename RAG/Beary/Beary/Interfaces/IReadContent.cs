using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Interfaces;

public interface IReadContent
{
    Task<Article> GetArticle(Identifier articleId);
    Task<IEnumerable<Article>> GetRelevantArticles(Vector embedding, TokenCount maxTokenCount);
}
