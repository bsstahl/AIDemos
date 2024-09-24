using Beary.ValueTypes;

namespace Beary.Entities;

public class Article
{
    public Identifier? Id { get; set; }
    public ArticleTitle? Title { get; set; }
    public ArticleContent? Content { get; set; }
    public TokenCount? TokenCount { get; set; }
    public IEnumerable<ContentChunk>? Chunks { get; set; }

    public Article(string id, string title, string content, int tokenCount)
        : this(id, title, content, tokenCount, null)
    { }

    public Article(string id, string title, string content, int tokenCount, IEnumerable<ContentChunk>? chunks)
        : this(Identifier.From(id), ArticleTitle.From(title), ArticleContent.From(content), TokenCount.From(tokenCount), chunks)
    { }

    public Article(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount, IEnumerable<ContentChunk>? chunks)
    {
        this.Id = id;
        this.Title = title;
        this.Content = content;
        this.TokenCount = tokenCount;
        this.Chunks = chunks;
    }
}
