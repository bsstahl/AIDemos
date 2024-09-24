using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Beary.Data.AzureAISearch.Extensions;
using Beary.ValueTypes;

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
            new SearchableField("Title") { IsFilterable = true, IsSortable = true },
            new SimpleField("TokenCount", SearchFieldDataType.Int32) { IsFilterable = true }
        };
    }

    internal async Task AddDocument(Document document)
    {
        var batch = IndexDocumentsBatch.Upload(new[] { document });
        await this.SearchClient.IndexDocumentsAsync(batch).ConfigureAwait(false);
    }

    internal async Task<Document> ReadById(Identifier id)
    {
        var result = await SearchClient.GetDocumentAsync<Document>(id.Value).ConfigureAwait(false);
        return result.Value;
    }

    internal async Task<bool> ArticleExists(Identifier id)
    {
        try
        {
            _ = await SearchClient
                .GetDocumentAsync<Document>(id.Value)
                .ConfigureAwait(false);

            return true;
        }
        catch (Azure.RequestFailedException)
        {
            return false;
        }
    }
}
