namespace Beary.Articles.FileSystem.Entities;

internal class BlogPostMetadata
{
    public bool isPublished => this.ispublished;

    public List<string>? tags { get; set; }
    public Guid id { get; set; }
    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public bool includeAlways { get; set; } = false;
    public List<string>? categories { get; set; }
    public int? menuorder  { get; set; }
    public string author { get; set; }
    public bool ispublished { get; set; }
    public bool showinlist { get; set; }
    public string publicationdate { get; set; }
    public string lastmodificationdate { get; set; }
    public string slug { get; set; }
    public bool buildifnotpublished { get; set; }
    public string teaser { get; set; }
}
