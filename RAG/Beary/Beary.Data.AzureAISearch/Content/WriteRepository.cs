using Beary.ValueTypes;

namespace Beary.Data.AzureAISearch.Content;

public class WriteRepository : IWriteContentSearchDocuments
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


    public WriteRepository(string searchServiceName, string apiKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(searchServiceName, nameof(searchServiceName));
        ArgumentException.ThrowIfNullOrEmpty(apiKey, nameof(apiKey));

        _searchServiceName = searchServiceName;
        _apiKey = apiKey;
    }

    public async Task SaveAsync(Identifier id, ArticleContent content, TokenCount tokenCount)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(content, nameof(content));
        ArgumentNullException.ThrowIfNull(tokenCount, nameof(tokenCount));

        var document = new Document
        {
            Id = id.Value,
            Content = content.Value,
            TokenCount = tokenCount.Value
        };

        await IndexClient.AddDocument(document).ConfigureAwait(false);
    }

}
