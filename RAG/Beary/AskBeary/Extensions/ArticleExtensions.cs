using Beary.Chat.Entities;
using Beary.Entities;

namespace AskBeary.Extensions;

internal static class ArticleExtensions
{
    internal static IEnumerable<ChatContent> AsChatContents(this IEnumerable<Article> articles, ChatRole role)
        => articles.Select(a => ChatContent.From(a.Content.Value, role));
}
