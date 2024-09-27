using Beary.Chat.Entities;
using Beary.Entities;

namespace Cluster.Extensions;

internal static class SearchResultExtensions
{
    internal static Double[][] AsEmbeddingsArray(this IEnumerable<SearchResult> searchResults)
        => searchResults.Select(sr 
            => sr.Embedding?.Select(e => Convert.ToDouble(e)).ToArray() ?? [])
        .ToArray();

    internal static IEnumerable<ChatContent> AsChatContents(this IEnumerable<SearchResult> searchResults, ChatRole role)
        => searchResults.Select(sr => sr.AsChatContent(role));

    internal static ChatContent AsChatContent(this SearchResult searchResult, ChatRole role)
        => ChatContent.From(searchResult.Content, role);
}
