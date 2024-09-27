namespace Beary.Embeddings.LocalServer.Extensions;

internal static class StringExtensions
{
    internal static string Sanitize(this string input)
    {
        return input.ToLowerInvariant().Trim();
    }

    internal static string[] Sanitize(this IEnumerable<string> input)
    {
        return input.Select(i => i.Sanitize()).ToArray();
    }
}
