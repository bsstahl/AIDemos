using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Beary.ValueTypes;

namespace Beary.Data.AzureAISearch;

public class WriteRepository : IWriteSearchDocuments
{
    private readonly string _searchServiceName;
    private readonly string _apiKey;

    private BearyIndex? _indexClient;
    private BearyIndex IndexClient
    {
        get
        {
            _indexClient ??= new BearyIndex(this.Endpoint, _apiKey);
            return _indexClient;
        }
    }

    public Uri Endpoint => new Uri($"https://{_searchServiceName}.search.windows.net");
    

    public WriteRepository(string searchServiceName, string apiKey)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(searchServiceName, nameof(searchServiceName));
        ArgumentNullException.ThrowIfNullOrEmpty(apiKey, nameof(apiKey));

        _searchServiceName = searchServiceName;
        _apiKey = apiKey;
    }

    public async Task SaveAsync(Identifier id, ArticleContent contentChunk, Location fullArticleLocation, Vector? embedding = null)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(fullArticleLocation, nameof(fullArticleLocation));
        ArgumentNullException.ThrowIfNull(contentChunk, nameof(contentChunk));

        var document = new SearchDocument
        {
            Id = id.Value,
            Content = contentChunk.Value,
            ContentSource = fullArticleLocation.Value,
            Vector = embedding?.Value ?? []
        };

        await this.IndexClient.AddDocument(document).ConfigureAwait(false);
    }
}
