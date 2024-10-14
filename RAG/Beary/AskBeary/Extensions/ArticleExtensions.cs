using Beary.Entities;

namespace AskBeary.Extensions;

internal static class ArticleExtensions
{
    internal static int GetTokenCount(this IEnumerable<Article> articles)
        => articles.Sum(a => a.TokenCount?.Value ?? 0);
}
