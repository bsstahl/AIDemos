using Azure.Search.Documents.Indexes;
using Beary.Entities;

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
            ItemId = this.ArticleId,
            ElementIndex = this.ElementIndex,
            Score = score,
            Content = this.Content,
            Embedding = this.Vector
        };
    }
}