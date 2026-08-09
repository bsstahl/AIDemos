using System.Text;
using System.Text.RegularExpressions;
using Tokenizer.Extensions;

namespace Tokenizer;

public class Model
{
    const string _regexPattern = @"(?i:'s|'t|'re|'ve|'m|'ll|'d)|[^\r\n\p{L}\p{N}]?\p{L}+|\p{N}{1,3}| ?[^\s\p{L}\p{N}]+[\r\n]*|\s*[\r\n]+|\s+(?!\S)|\s+";
    const string _specialTokenPattern = @"<\|[^|]+\|>";

    // cl100k_base special tokens (subset used here)
    static readonly IDictionary<string, int> _specialTokens = new Dictionary<string, int>
    {
        { "<|endoftext|>",   100257 },
        { "<|fim_prefix|>",  100258 },
        { "<|fim_middle|>",  100259 },
        { "<|fim_suffix|>",  100260 },
        { "<|endofprompt|>", 100276 },
    };

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

    public IEnumerable<int> Encode(string text, ISet<string>? allowedSpecialTokens = null, ISet<string>? disallowedSpecialTokens = null)
    {
        // Check for disallowed special tokens
        if (disallowedSpecialTokens is not null)
        {
            foreach (var token in disallowedSpecialTokens)
            {
                if (!string.IsNullOrEmpty(token) && text.Contains(token, StringComparison.Ordinal))
                    throw new ArgumentException($"Disallowed special token found in input: {token}", nameof(text));
            }
        }

        // Split text on allowed special tokens; throw on any unallowed special tokens
        var result = new List<int>();
        int lastIndex = 0;

        foreach (Match match in Regex.Matches(text, _specialTokenPattern, RegexOptions.CultureInvariant))
        {
            var token = match.Value;

            if (!_specialTokens.ContainsKey(token))
                continue; // Not a known special token — treat as regular text

            if (allowedSpecialTokens is null || !allowedSpecialTokens.Contains(token))
                throw new ArgumentException($"Special token not allowed in input: {token}", nameof(text));

            // Encode the text segment before this special token
            var segment = text[lastIndex..match.Index];
            if (!string.IsNullOrEmpty(segment))
                result.AddRange(segment.AsSegments(_regexPattern, this.TextValues).SelectMany(s => s.Encode()));

            result.Add(_specialTokens[token]);
            lastIndex = match.Index + match.Length;
        }

        // Encode any remaining text after the last special token
        var remainder = text[lastIndex..];
        if (!string.IsNullOrEmpty(remainder))
            result.AddRange(remainder.AsSegments(_regexPattern, this.TextValues).SelectMany(s => s.Encode()));

        return result;
    }

}
