using System.Text;

namespace Tokenizer;

internal class Segment
{
    private readonly IDictionary<byte[], int> _replacementsByText;
    private readonly string _text;

    public Segment(IDictionary<byte[], int> replacementsByText, string text)
    {
        _replacementsByText = replacementsByText;
        _text = text;
    }

    internal IEnumerable<int> Encode()
    {
        var result = new List<int>();

        var utf8Bytes = Encoding.UTF8.GetBytes(_text);
        if (utf8Bytes is null || !utf8Bytes.Any())
            throw new InvalidOperationException("Key cannot be null or empty");
        else if (_replacementsByText.TryGetValue(utf8Bytes, out var token))
            result.Add(token); // Already a single token
        else
        {
            // Start with each byte as its own partition and merge them based on rank
            var partitions = new PartitionCollection(_replacementsByText, utf8Bytes);
            result.AddRange(partitions.Encode()); // Encode the resulting partitions
        }

        return result;
    }


}
