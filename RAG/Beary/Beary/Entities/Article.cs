using Beary.ValueTypes;

namespace Beary.Entities;

public class Article
{
    public Identifier Id { get; set; }
    public Location SourceLocaton { get; set; }
    public ArticleContent Content { get; set; }
}
