using Beary.ValueTypes;

namespace Beary.Entities;

public class ContentChunk
{
    public Identifier Id { get; set; }
    public ElementIndex Index { get; set; }
    public ArticleContent ChunkText { get; set; }
    public Vector? Embedding { get; set; }

    public ContentChunk(string id, int index, string chunkText, IEnumerable<Single>? embedding)
        : this(Identifier.From(id), ElementIndex.From(index), ArticleContent.From(chunkText), Vector.From(embedding))
    { }

    public ContentChunk(Identifier id, ElementIndex index, ArticleContent chunkText, Vector? embedding)
    {
        this.Id = id;
        this.Index = index;
        this.ChunkText = chunkText;
        this.Embedding = embedding;
    }
}
