using System.Text.RegularExpressions;

namespace Tokenizer;

internal class SegmentCollection
{
    Regex _regex;
    IDictionary<byte[], int> _replacementsByText { get; }

    IEnumerable<Segment> _segments;

    public SegmentCollection(IDictionary<byte[], int> replacementsByText, string regexPattern, string text)
    { 
        _replacementsByText = replacementsByText;
        _regex = new Regex(regexPattern);
        _segments = string.IsNullOrEmpty(text)
            ? new List<Segment>()
            : _regex.Matches(text)
                .Select(m => new Segment(_replacementsByText, m.Value));
    }

    internal IEnumerable<int> Encode()
    {
        return _segments.SelectMany(s => s.Encode());
    }

}
