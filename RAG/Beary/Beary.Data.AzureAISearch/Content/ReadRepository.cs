﻿using Beary.Application.Interfaces;
using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Data.AzureAISearch.Content;

public class ReadRepository : IReadContentSearchDocuments
{
    // https://[search_service_name].search.windows.net/indexes/[index_name]/docs/[document_key]?api-version=[api_version]
    // https://beary-search.search.windows.net/indexes/beary-content-index/docs/d53796a4-61dc-4dcf-94b2-44813875826a?api-version=2024-05-01-preview

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

    public async Task<Article> GetArticle(string articleId)
    {
        ArgumentNullException.ThrowIfNull(articleId, nameof(articleId));
        var id = Identifier.From(articleId);

        var results = await IndexClient.ReadById(id).ConfigureAwait(false);
        return new Article(id.Value, results.Title, results.Content, results.TokenCount);
    }

    public async Task<bool> ArticleExists(string articleId)
    {
        ArgumentNullException.ThrowIfNull(articleId, nameof(articleId));
        var id = Identifier.From(articleId);

        return await IndexClient.ArticleExists(id).ConfigureAwait(false);
    }

}
