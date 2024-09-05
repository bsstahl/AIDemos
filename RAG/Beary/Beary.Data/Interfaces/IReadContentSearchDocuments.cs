using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Data.Interfaces;

public interface IReadContentSearchDocuments
{
    Task<Article> GetArticle(Identifier articleId);
}
