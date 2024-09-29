using Beary.Entities;

namespace Beary.Documents.Extensions;

internal static class ContentChunkExtensions
{
    internal static bool IsPopulated(this ContentChunk? chunk)
    {
        return chunk is not null && chunk.Embedding is not null;
    }
}
