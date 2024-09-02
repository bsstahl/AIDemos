using Beary.Builders;
using System.Text;
using TestHelperExtensions;

namespace Beary.Data.Test.Extensions;

internal static class ChunkBuilderExtensions
{
    const int vectorSearchDimensions = 3;

    internal static ContentChunkBuilder UseRandomValues(this ContentChunkBuilder builder, int index)
    {
        var vector = new List<Single>();
        for (var i = 0; i < vectorSearchDimensions; i++)
            vector.Add((1.0f).GetRandom(0.0f));

        return builder
            .Id(Guid.NewGuid().ToString())
            .Index(index)
            .ChunkText(string.Empty.GetRandom())
            .Vector(vector);
    }

    internal static string BuildArticleContent(this IEnumerable<ContentChunkBuilder> builders)
    {
        return new StringBuilder()
            .AppendLine($"# {string.Empty.GetRandom()}")
            .AppendLine()
            .AppendLine(string.Join("\r\n\r\n", builders.OrderBy(b => b.ItemIndex).Select(b => b.ContentChunkText)))
            .ToString();
    }
}
