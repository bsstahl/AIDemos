using Beary.ValueTypes;

namespace Beary.Data.Entities;

public class Article
{
    public Identifier? Id { get; set; }
    public ArticleContent? Content { get; set; }
    public TokenCount? TokenCount { get; set; }
    public IEnumerable<ContentChunk>? Chunks { get; set; }

    public Article(string id, string content, int tokenCount)
        : this(id, content, tokenCount, null)
    { }

    public Article(string id, string content, int tokenCount, IEnumerable<ContentChunk>? chunks)
        : this(Identifier.From(id), ArticleContent.From(content), TokenCount.From(tokenCount), chunks)
    { }

    public Article(Identifier id, ArticleContent content, TokenCount tokenCount, IEnumerable<ContentChunk>? chunks)
    {
        this.Id = id;
        this.Content = content;
        this.TokenCount = tokenCount;
        this.Chunks = chunks;
    }
}
