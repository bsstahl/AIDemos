using Beary.Entities;
using Beary.Extensions;
using System.Security.Principal;

namespace Beary.Builders;

public class ContentChunkBuilder
{
    private string? _id;
    private int? _index;
    private string? _chunkText;
    private IEnumerable<Double>? _vector;

    public ContentChunk Build()
    {
        _id.ThrowIfNull(nameof(_id));
        _index.ThrowIfNull(nameof(_index));
        _chunkText.ThrowIfNull(nameof(_chunkText));

        return new ContentChunk(_id!, _index!.Value, _chunkText!, _vector);
    }

    public string? ContentChunkText => _chunkText;
    public int? ItemIndex => _index;


    public ContentChunkBuilder Id(string id)
    {
        _id = id;
        return this;
    }

    public ContentChunkBuilder Index(int index)
    {
        _index = index;
        return this;
    }

    public ContentChunkBuilder ChunkText(string chunkText)
    {
        _chunkText = chunkText;
        return this;
    }

    public ContentChunkBuilder Vector(IEnumerable<Double> vector)
    {
        _vector = vector;
        return this;
    }
}
