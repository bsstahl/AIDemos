using Azure.Search.Documents.Indexes;

namespace Beary.Data.AzureAISearch;

internal class SearchDocument
{
    [SimpleField(IsKey = true)]
    public string Id { get; set; }

    [SearchableField]
    public string Content { get; set; }

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public Uri ContentSource { get; set; }

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public IEnumerable<Double>? Vector { get; set; }
}