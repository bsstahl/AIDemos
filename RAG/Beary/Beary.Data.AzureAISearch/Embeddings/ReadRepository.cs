using Beary.Data.Interfaces;
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

    public async Task<IEnumerable<Beary.Data.Entities.SearchResult>> GetNearestNeighbors(Vector queryVector, ResultCount numberOfNeighbors)
    {
        ArgumentNullException.ThrowIfNull(queryVector, nameof(queryVector));
        ArgumentNullException.ThrowIfNull(numberOfNeighbors, nameof(numberOfNeighbors));

        return await IndexClient
            .GetNearestNeighbors(queryVector, numberOfNeighbors)
            .ConfigureAwait(false);
    }
}
