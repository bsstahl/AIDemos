using Beary.Documents.Interfaces;
using Beary.ValueTypes;

namespace Beary.Data.Db.Repositories;

internal sealed class EmbeddingsWriteRepository : IWriteEmbeddingsSearchDocuments
{
    private readonly IBearyDataStore _dataStore;

    public EmbeddingsWriteRepository(IBearyDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task SaveAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding)
        => _dataStore.SaveEmbeddingAsync(id, elementIndex, contentChunk, fullArticleId, embedding);
}
