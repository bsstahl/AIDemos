namespace Beary.Data.Entities;

public class SearchResult
{
    public string Id { get; set; }
    public int ElementIndex { get; set; }
    public string Content { get; set; }
    public string ArticleId { get; set; }
    public IEnumerable<double>? Vector { get; set; }
    public double Distance { get; set; }

}
