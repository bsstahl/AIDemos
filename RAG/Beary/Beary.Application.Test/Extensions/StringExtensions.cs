namespace Beary.Application.Test.Extensions;

[ExcludeFromCodeCoverage]
internal static class StringExtensions
{
    internal static IEnumerable<float> GetTextEmbedding(this string text)
        => BitConverter.GetBytes(text.GetHashCode()).Select(b => (float)b / 255.0f).ToArray();
}
