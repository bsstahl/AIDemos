using Azure.Search.Documents.Indexes;
using Beary.Data.Entities;

namespace Beary.Data.AzureAISearch.Embeddings;

internal class Document
{
    [SimpleField(IsKey = true)]
    public string Id { get; set; }

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public int ElementIndex { get; set; }

    [SearchableField]
    public string Content { get; set; }

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public string ArticleId { get; set; }

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public IEnumerable<Single>? Vector { get; set; }


    public SearchResult AsSearchResult(double score)
    {
        return new SearchResult()
        {
            Id = this.Id,
            ArticleId = this.ArticleId,
            ElementIndex = this.ElementIndex,
            Distance = score,
            Content = this.Content
        };
    }
}