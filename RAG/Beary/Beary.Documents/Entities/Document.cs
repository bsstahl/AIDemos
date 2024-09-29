namespace Beary.Documents.Entities;

public class Document
{
    public string Id { get; set; }
    public string Title { get; set; }

    public string FullText { get; set; }
    public IEnumerable<string> ContentChunks { get; set; }

}
