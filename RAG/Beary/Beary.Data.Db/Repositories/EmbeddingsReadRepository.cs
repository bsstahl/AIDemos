using Beary.Documents.Interfaces;
using Beary.Entities;

namespace Beary.Data.Db.Repositories;

internal sealed class EmbeddingsReadRepository : IReadEmbeddingsSearchDocuments
{
    private readonly IBearyDataStore _dataStore;

    public EmbeddingsReadRepository(IBearyDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<long> GetDocumentCount() => _dataStore.GetEmbeddingDocumentCountAsync();

    public Task<IEnumerable<SearchResult>> GetNearestNeighbors(IEnumerable<float> queryVector, int numberOfNeighbors)
        => _dataStore.GetNearestNeighborsAsync(queryVector, numberOfNeighbors);

    public Task<IEnumerable<SearchResult>> GetAllEmbeddings() => _dataStore.GetAllEmbeddingsAsync();
}
