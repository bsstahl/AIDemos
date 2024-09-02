using Beary.Builders;
using TestHelperExtensions;

namespace Beary.Data.Test.Extensions;

internal static class ArticleBuilderExtensions
{
    internal static ArticleBuilder UseRandomValues(this ArticleBuilder builder)
    {
        var chunkBuilders = new List<ContentChunkBuilder>();
        for (int i = 0; i < 10.GetRandom(4); i++)
            chunkBuilders.Add(new ContentChunkBuilder().UseRandomValues(i));

        var content = chunkBuilders.BuildArticleContent();

        return builder
            .Id(Guid.NewGuid().ToString())
            .Content(content)
            .TokenCount(Int16.MaxValue.GetRandom())
            .AddContentChunks(chunkBuilders);
    }
}
