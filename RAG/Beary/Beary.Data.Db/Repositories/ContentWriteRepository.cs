using Beary.Documents.Interfaces;
using Beary.ValueTypes;

namespace Beary.Data.Db.Repositories;

internal sealed class ContentWriteRepository : IWriteContentSearchDocuments
{
    private readonly IBearyDataStore _dataStore;

    public ContentWriteRepository(IBearyDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task SaveAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount)
        => _dataStore.SaveArticleAsync(id, title, content, tokenCount);
}
