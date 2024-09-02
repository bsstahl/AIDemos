using Azure.Search.Documents.Indexes;
using Azure.Search.Documents;
using Azure;
using Azure.Search.Documents.Indexes.Models;

namespace Beary.Data.AzureAISearch.Extensions;

internal static class SearchIndexExtensions
{
    internal static SearchClient GetSearchClient(this SearchIndex index, Uri endpoint, string apiKey)
    {
        var credential = new AzureKeyCredential(apiKey);

        var indexClient = new SearchIndexClient(endpoint, credential);
        var indexTask = indexClient.CreateOrUpdateIndexAsync(index);
        indexTask.Wait();

        var client = new SearchClient(endpoint, index.Name, credential);

        return client;
    }

}
