using System.Text.RegularExpressions;

namespace Tokenizer.Extensions;

internal static class StringExtensions
{
    internal static IEnumerable<Segment> AsSegments(this string text, string regexPattern, IDictionary<byte[], int> textValues)
    {
        return string.IsNullOrEmpty(text)
            ? new List<Segment>()
            : new Regex(regexPattern).Matches(text)
                .Select(m => new Segment(textValues, m.Value));
    }
}
