using Beary.Builders;
using System.Text;
using TestHelperExtensions;

namespace Beary.Data.Test.Extensions;

internal static class ChunkBuilderExtensions
{
    internal static ContentChunkBuilder UseRandomValues(this ContentChunkBuilder builder, int index)
    {
        var vector = new List<double>();
        for (var i = 0; i < 7.GetRandom(3); i++)
            vector.Add((1.0).GetRandom(0.0));

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
