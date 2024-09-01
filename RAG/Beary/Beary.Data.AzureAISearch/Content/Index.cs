using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Beary.Data.AzureAISearch.Extensions;

namespace Beary.Data.AzureAISearch.Content;

internal class Index : SearchIndex
{
    // TODO: Get From Config
    const string indexName = "beary-content-index";

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

        this.Fields = new List<SearchField>()
        {
            new SimpleField("Id", SearchFieldDataType.String) { IsKey = true },
            new SearchableField("Content") { IsFilterable = true, IsSortable = true },
            new SimpleField("TokenCount", SearchFieldDataType.Int32) { IsFilterable = true }
        };
    }

    internal async Task AddDocument(Document document)
    {
        var batch = IndexDocumentsBatch.Upload(new[] { document });
        await SearchClient.IndexDocumentsAsync(batch).ConfigureAwait(false);
    }

}
