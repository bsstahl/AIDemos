using System;
using System.Text;
using System.Text.RegularExpressions;
using Tokenizer.Extensions;

namespace Tokenizer;

public class Model
{
    const string _regex = @"(?i:'s|'t|'re|'ve|'m|'ll|'d)|[^\r\n\p{L}\p{N}]?\p{L}+|\p{N}{1,3}| ?[^\s\p{L}\p{N}]+[\r\n]*|\s*[\r\n]+|\s+(?!\S)|\s+";

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
        var result = new List<int>();

        if (!string.IsNullOrEmpty(text))
        {
            new Regex(_regex)
                .Matches(text).ToList()
                .ForEach(m => result.AddRange(EncodeSegment(m.Value)));
        }

        return result;
    }

    private IEnumerable<int> EncodeSegment(string text)
    {
        var result = new List<int>();

        var utf8Bytes = Encoding.UTF8.GetBytes(text);
        if (utf8Bytes is null || !utf8Bytes.Any())
            throw new InvalidOperationException("Key cannot be null or empty");
        else if (this.TextValues.TryGetValue(utf8Bytes, out var token))
            result.Add(token); // Already a single token
        else
        {
            // Start with each byte as its own partition and merge them based on rank
            var partitions = new PartitionCollection(this.TextValues, utf8Bytes);
            result.AddRange(partitions.Encode()); // Encode the resulting partitions
        }

        return result;
    }

}
