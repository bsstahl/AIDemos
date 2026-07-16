using Beary.Documents.Interfaces;
using Beary.Entities;

namespace Beary.Data.Db.Repositories;

internal sealed class ContentReadRepository : IReadContentSearchDocuments
{
    private readonly IBearyDataStore _dataStore;

    public ContentReadRepository(IBearyDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Article> GetArticle(string articleId) => _dataStore.GetArticleAsync(articleId);

    public Task<bool> ArticleExists(string articleId) => _dataStore.ArticleExistsAsync(articleId);
}
