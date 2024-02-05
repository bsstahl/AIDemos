namespace Tokenizer;

internal class Partition
{
    // public int Index { get; set; }
    public IEnumerable<byte> Value { get; set; }
    public int? Rank { get; set; }

    private readonly IDictionary<byte[], int> _replacementsByText;

    // int index, 
    public Partition(IDictionary<byte[], int> replacementsByText, IEnumerable<byte> utf8Bytes)
    {
        _replacementsByText = replacementsByText;
        // this.Index = index;
        this.Value = utf8Bytes;
        this.Rank = GetRank();
    }

    internal int? GetRank()
    {
        return _replacementsByText
            .TryGetValue(this.Value.ToArray(), out var rank) ? rank : (int?)null;
    }

}
