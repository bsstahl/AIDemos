using Azure.Search.Documents.Indexes;

namespace Beary.Data.AzureAISearch.Content;

internal class Document
{
    [SimpleField(IsKey = true)]
    public string Id { get; set; }

    [SearchableField]
    public string Title { get; set; }

    [SearchableField]
    public string Content { get; set; }

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public Int32 TokenCount { get; set; }
}