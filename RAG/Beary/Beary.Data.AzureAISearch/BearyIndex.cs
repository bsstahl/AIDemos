using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace Beary.Data.AzureAISearch;

internal class BearyIndex : SearchIndex
{
    // TODO: Get From Config
    const string indexName = "beary-index";

    private SearchClient? _searchClient;
    internal SearchClient SearchClient 
    {
        get
        {
            _searchClient ??= this.GetSearchClient();
            return _searchClient;
        } 
    }

    internal Uri Endpoint { get; private set; }
    internal string ApiKey { get; private set; }


    internal BearyIndex(Uri endpoint, string apiKey) : base(indexName)
    {
        this.Endpoint = endpoint;
        this.ApiKey = apiKey;

        base.Fields = new List<SearchField>()
        {
            new SimpleField("Id", SearchFieldDataType.String) { IsKey = true },
            new SearchableField("Content") { IsFilterable = true, IsSortable = true },
            new SimpleField("Vector", SearchFieldDataType.Collection(SearchFieldDataType.Double)) { IsFilterable = true },
            new SimpleField("ContentSource", SearchFieldDataType.String) { IsFilterable = true, IsSortable = true }
        };
    }

    internal async Task AddDocument(SearchDocument document)
    {
        var batch = IndexDocumentsBatch.Upload(new[] { document });
        await this.SearchClient.IndexDocumentsAsync(batch).ConfigureAwait(false);
    }

    internal SearchClient GetSearchClient()
    {
        var credential = new AzureKeyCredential(this.ApiKey);

        var indexClient = new SearchIndexClient(this.Endpoint, credential);
        var indexTask = indexClient
            .CreateOrUpdateIndexAsync(this);
        indexTask.Wait();

        return new SearchClient(this.Endpoint, this.Name, credential);
    }
}
