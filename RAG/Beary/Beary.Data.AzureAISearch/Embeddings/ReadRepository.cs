using Beary.Documents.Interfaces;
using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Data.AzureAISearch.Embeddings;

public class ReadRepository : IReadEmbeddingsSearchDocuments
{
    private readonly string _searchServiceName;
    private readonly string _apiKey;

    private Index? _indexClient;
    private Index IndexClient
    {
        get
        {
            _indexClient ??= new Index(Endpoint, _apiKey);
            return _indexClient;
        }
    }

    public Uri Endpoint => new Uri($"https://{_searchServiceName}.search.windows.net");

    public ReadRepository(string searchServiceName, string apiKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(searchServiceName, nameof(searchServiceName));
        ArgumentException.ThrowIfNullOrEmpty(apiKey, nameof(apiKey));

        _searchServiceName = searchServiceName;
        _apiKey = apiKey;
    }

    public async Task<IEnumerable<Beary.Entities.SearchResult>> GetNearestNeighbors(IEnumerable<float> queryVector, int neighborCount)
    {
        ArgumentNullException.ThrowIfNull(queryVector, nameof(queryVector));
        
        var vector = Vector.From(queryVector);
        var numberOfNeighbors = ResultCount.From(neighborCount);

        return await IndexClient
            .GetNearestNeighbors(vector, numberOfNeighbors)
            .ConfigureAwait(false);
    }

    public async Task<long> GetDocumentCount()
    {
        return await this.IndexClient
            .GetDocumentCount()
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SearchResult>> GetAllEmbeddings()
    {
        return await this.IndexClient
            .GetAllEmbeddings()
            .ConfigureAwait(false);
    }
}
