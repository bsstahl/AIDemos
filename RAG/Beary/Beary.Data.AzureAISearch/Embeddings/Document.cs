using Azure.Search.Documents.Indexes;

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
    public IEnumerable<double>? Vector { get; set; }
}