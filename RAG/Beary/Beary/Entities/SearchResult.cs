namespace Beary.Entities;

public class SearchResult
{
    public string Id { get; set; }
    public int ElementIndex { get; set; }
    public string Content { get; set; }
    public string ItemId { get; set; }
    public double Score { get; set; }
    public IEnumerable<Single>? Embedding { get; set; }
}
