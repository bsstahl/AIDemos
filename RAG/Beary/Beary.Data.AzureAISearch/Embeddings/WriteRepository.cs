using Beary.Documents.Interfaces;
using Beary.ValueTypes;

namespace Beary.Data.AzureAISearch.Embeddings;

public class WriteRepository : IWriteEmbeddingsSearchDocuments
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

    public async Task SaveAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding = null)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(elementIndex, nameof(elementIndex));
        ArgumentNullException.ThrowIfNull(fullArticleId, nameof(fullArticleId));
        ArgumentNullException.ThrowIfNull(contentChunk, nameof(contentChunk));

        var document = new Document
        {
            Id = id.Value,
            ElementIndex = elementIndex.Value,
            Content = contentChunk.Value,
            ArticleId = fullArticleId.Value,
            Vector = embedding?.Value ?? []
        };

        await this.IndexClient.AddDocument(document).ConfigureAwait(false);
        Console.WriteLine(document.Id + " added to index.");
    }
}
