using System.Text;
using System.Text.RegularExpressions;

namespace Tokenizer;

public class Model
{
    const string _regexPattern = @"(?i:'s|'t|'re|'ve|'m|'ll|'d)|[^\r\n\p{L}\p{N}]?\p{L}+|\p{N}{1,3}| ?[^\s\p{L}\p{N}]+[\r\n]*|\s*[\r\n]+|\s+(?!\S)|\s+";

    IDictionary<int, byte[]>? _tokens;
    IDictionary<byte[], int>? _textValues;

    private Replacements? _replacements;
    private Replacements Replacements
    {
        get
        {
            _replacements ??= new Replacements();
            return _replacements;
        }
    }

    public IDictionary<int, byte[]> Tokens
    {
        get
        {
            _tokens ??= this.Replacements.GetReplacementsByToken();
            return _tokens;
        }
    }

    public IDictionary<byte[], int> TextValues
    {
        get
        {
            _textValues ??= this.Replacements.GetReplacementsByText();
            return _textValues;
        }
    }

    public string Decode(IEnumerable<int> tokens)
    {
        return (tokens is null || !tokens.Any())
            ? string.Empty
            : Encoding.UTF8.GetString(tokens.SelectMany(t => this.Tokens[t]).ToArray());
    }

    public IEnumerable<int> Encode(string text, ISet<string>? _1 = null, ISet<string>? _2 = null)
    {
        var segments = string.IsNullOrEmpty(text)
            ? new List<Segment>()
            : new Regex(_regexPattern).Matches(text)
                .Select(m => new Segment(this.TextValues, m.Value));
        return segments.SelectMany(s => s.Encode());
    }

}
