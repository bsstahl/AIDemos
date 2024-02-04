using Tokenizer.Extensions;

namespace Tokenizer;

internal class PartitionCollection
{
    IDictionary<byte[], int> _replacementsByText;
    IEnumerable<byte> _utf8Bytes;

    List<(int Start, int Rank)> _partitions;

    public int Count { get => _partitions.Count; }
    public bool IsMergeable { get => (_partitions.Count > 1 && (this.LowestRank < int.MaxValue));  }
    public int LowestRank { get => _partitions.Min(p => p.Rank); }
    public int LowestRankIndex
    {
        get => _partitions.IndexOf(_partitions.First(p => p.Rank == this.LowestRank));
    }


    public PartitionCollection(IDictionary<byte[], int> replacementsByText, IEnumerable<byte> utf8Bytes)
    {
        _utf8Bytes = utf8Bytes;
        _replacementsByText = replacementsByText;
        _partitions = _utf8Bytes.AsPartitions().ToList();

        // Get ranks for each pair of partitions
        for (var i = 0; i < _partitions.Count - 2; i++)
        {
            var rank = GetRank(_utf8Bytes, i, 0);
            if (rank.HasValue)
                _partitions[i] = (_partitions[i].Start, rank.Value);
        }

        // Merge partitions based on rank
        while (this.IsMergeable)
        {
            this.MergeSequential(this.LowestRankIndex);
        }
    }

    internal IEnumerable<int> Encode()
    {
        // Get the tokens for each remaining partition
        var output = new List<int>(this.Count - 1);
        for (var i = 0; i < this.Count - 1; i++)
        {
            var start = _partitions[i].Start;
            var end = _partitions[i + 1].Start;
            var partitionKey = _utf8Bytes.Skip(start).Take(end - start).ToArray();
            var tokens = _replacementsByText[partitionKey];
            output.Add(tokens);
        }

        return output;
    }

    internal void MergeSequential(int rankIndex)
    {
        var rank = GetRank(_utf8Bytes, rankIndex, 1);
        _partitions[rankIndex] = (_partitions[rankIndex].Start, rank ?? int.MaxValue);
        if (rankIndex > 0)
        {
            var newRank = GetRank(_utf8Bytes, rankIndex - 1, 1);
            _partitions[rankIndex - 1] = (_partitions[rankIndex - 1].Start, newRank ?? int.MaxValue);
        }
        _partitions.RemoveAt(rankIndex + 1);
    }

    internal int? GetRank(IEnumerable<byte> inputBytes, int startIndex, int skip)
    {
        int? result = null;

        if (startIndex + skip + 2 < _partitions.Count)
        {
            var start = _partitions[startIndex].Start;
            var length = _partitions[startIndex + skip + 2].Start - _partitions[startIndex].Start;
            var key = inputBytes.Skip(start).Take(length).ToArray();
            result = _replacementsByText.TryGetValue(key, out var rank) ? rank : (int?)null;
        }

        return result;
    }


}
