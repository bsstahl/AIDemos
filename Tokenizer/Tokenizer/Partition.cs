namespace Tokenizer;

internal class Partition
{
    public IEnumerable<byte> Value { get; set; }
    public int? RankWithNext { get => GetRank(); }

    public Partition? Next { get; internal set; }


    private readonly IDictionary<byte[], int> _replacementsByText;

    public Partition(IDictionary<byte[], int> replacementsByText, IEnumerable<byte> utf8Bytes, Partition? next)
    {
        _replacementsByText = replacementsByText;
        this.Value = utf8Bytes;
        this.Next = next;
    }

    internal int Encode()
        => _replacementsByText[this.Value.ToArray()];

    private int? GetRank()
    {
        return (this.Next is null) 
            ? null
            : _replacementsByText.TryGetValue(this.Value.Concat(this.Next.Value).ToArray(), out var rank) 
                ? rank 
                : (int?)null;
    }

}
