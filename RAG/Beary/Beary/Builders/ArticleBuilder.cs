using Beary.Entities;
using Beary.Extensions;
using Beary.ValueTypes;

namespace Beary.Builders;

public class ArticleBuilder
{
    private string? _id;
    private string? _content;
    private int? _tokenCount;

    private List<ContentChunkBuilder>? _chunkBuilders;
    private List<ContentChunkBuilder> ChunkBuilders
        => _chunkBuilders ??= new List<ContentChunkBuilder>();

    public Article Build()
    {
        _id.ThrowIfNull(nameof(_id));
        _content.ThrowIfNull(nameof(_content));
        _tokenCount.ThrowIfNull(nameof(_tokenCount));

        var chunks = _chunkBuilders?.Select(x => x.Build());
        return new Article(_id!, _content!, _tokenCount!.Value, chunks);
    }

    public ArticleBuilder Id(string id)
    {
        _id = id;
        return this;
    }

    public ArticleBuilder Content(string content)
    {
        _content = content;
        return this;
    }

    public ArticleBuilder TokenCount(int tokenCount)
    {
        _tokenCount = tokenCount;
        return this;
    }

    public ArticleBuilder AddContentChunk(string id, int index, string chunkText, IEnumerable<Single>? vector)
    {
        var builder = new ContentChunkBuilder()
                .Id(id)
                .Index(index)
                .ChunkText(chunkText);
        if (vector is not null) builder.Vector(vector);
        return this.AddContentChunk(builder);
    }

    public ArticleBuilder AddContentChunk(ContentChunkBuilder chunkBuilder)
    {
        return this.AddContentChunks(new[] { chunkBuilder });
    }

    public ArticleBuilder AddContentChunks(IEnumerable<ContentChunkBuilder> chunkBuilders)
    {
        this.ChunkBuilders.AddRange(chunkBuilders);
        return this;
    }
}
