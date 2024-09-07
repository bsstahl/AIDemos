using Beary.Chat.Entities;
using Beary.Entities;

namespace AskBeary.Extensions;

internal static class ArticleExtensions
{
    internal static IEnumerable<ChatContent> AsChatContents(this IEnumerable<Article> articles, ChatRole role)
        => articles.Select(a => ChatContent.From(a.Content.Value, role));

    internal static int GetTokenCount(this IEnumerable<Article> articles)
        => articles.Sum(a => a.TokenCount?.Value ?? 0);
}
