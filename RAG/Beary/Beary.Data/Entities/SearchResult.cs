using System.Text.Json;

namespace Beary.Data.Entities;

public class SearchResult
{
    public string Id { get; set; }
    public int ElementIndex { get; set; }
    public string Content { get; set; }
    public string ArticleId { get; set; }
    public double Score { get; set; }
    public IEnumerable<Single>? Embedding { get; set; }


    override public string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
