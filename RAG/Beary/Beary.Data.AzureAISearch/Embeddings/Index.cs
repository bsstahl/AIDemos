using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Beary.Data.AzureAISearch.Extensions;
using Beary.ValueTypes;

namespace Beary.Data.AzureAISearch.Embeddings;

internal class Index : SearchIndex
{
    // TODO: Get From Config
    const string indexName = "beary-embeddings-index";
    const int vectorSearchDimensions = 768;
    const string vectorSearchProfileName = "gptSearchProfile";

    private SearchClient? _searchClient;
    internal SearchClient SearchClient
    {
        get
        {
            _searchClient ??= this.GetSearchClient(this.Endpoint, this.ApiKey);
            return _searchClient;
        }
    }

    internal Uri Endpoint { get; private set; }
    internal string ApiKey { get; private set; }


    internal Index(Uri endpoint, string apiKey) : base(indexName)
    {
        this.Endpoint = endpoint;
        this.ApiKey = apiKey;

        var vectorSearchAlgorithm = new HnswAlgorithmConfiguration("hnsw");
        this.VectorSearch = new VectorSearch();
        this.VectorSearch.Algorithms.Add(vectorSearchAlgorithm);
        this.VectorSearch.Profiles.Add(new VectorSearchProfile(vectorSearchProfileName, "hnsw"));

        this.Fields = new List<SearchField>()
        {
            new SimpleField("Id", SearchFieldDataType.String) { IsKey = true },
            new SearchableField("Content") { IsFilterable = true, IsSortable = true },
            new VectorSearchField("Vector", vectorSearchDimensions, vectorSearchProfileName),
            new SimpleField("ArticleId", SearchFieldDataType.String) { IsFilterable = false, IsSortable = false },
            new SimpleField("ElementIndex", SearchFieldDataType.Int32) { IsSortable = true }
        };
    }

    internal async Task AddDocument(Document document)
    {
        var batch = IndexDocumentsBatch.Upload(new[] { document });
        await this.SearchClient.IndexDocumentsAsync(batch).ConfigureAwait(false);
    }

    internal async Task<long> GetDocumentCount()
    {
        var stats = await SearchClient.GetDocumentCountAsync().ConfigureAwait(false);
        return stats.Value;
    }

    internal async Task<IEnumerable<Entities.SearchResult>> GetAllEmbeddings()
    {
        var searchOptions = new SearchOptions
        {
            IncludeTotalCount = true,
            Size = 5000,
            QueryType = SearchQueryType.Full
        };

        var queryResponse = await SearchClient.SearchAsync<Document>("*", searchOptions).ConfigureAwait(false);
        var pagedResults = queryResponse.Value.GetResults();
        return pagedResults.Select(r => r.Document.AsSearchResult(r.Score ?? 0.0)).ToList();
    }

    internal async Task<IEnumerable<Entities.SearchResult>> GetNearestNeighbors(Vector queryVector, ResultCount numberOfNeighbors)
    {
        var searchOptions = new SearchOptions
        {
            IncludeTotalCount = true,
            Size = numberOfNeighbors.Value,
            QueryType = SearchQueryType.Full,
            VectorSearch = new VectorSearchOptions()
        };

        var searchCriteria = new VectorizedQuery(new ReadOnlyMemory<Single>(queryVector.Value.ToArray()));
        searchCriteria.Fields.Add("Vector");
        searchOptions.VectorSearch.Queries.Add(searchCriteria);

        using var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        var queryResponse = await this.SearchClient
            .SearchAsync<Document>("", searchOptions, cancellationToken)
            .ConfigureAwait(false);

        var pagedResults = queryResponse.Value.GetResults();
        return pagedResults.Select(r => r.Document.AsSearchResult(r.Score ?? 0.0)).ToList();
    }

}
