using Beary.ValueTypes;

namespace Beary.Data.Entities;

public class ContentChunk
{
    public Identifier? Id { get; set; }
    public ElementIndex? Index { get; set; }
    public ArticleContent? ChunkText { get; set; }
    public Vector? Embedding { get; set; }
}
