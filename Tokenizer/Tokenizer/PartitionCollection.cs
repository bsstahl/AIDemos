namespace Tokenizer;

internal class PartitionCollection
{
    IDictionary<byte[], int> _replacementsByText;
    List<Partition> _partitions;

    private int? LowestPairRank 
    { 
        get => _partitions.Min(p => p.RankWithNext); 
    }

    private Partition? LowestRankedPartition
    {
        get => _partitions.FirstOrDefault(p => p.RankWithNext == this.LowestPairRank);
    }

    private bool HasMergeablePartitions 
    { 
        get => (_partitions.Count > 1 && (this.LowestPairRank is not null)); 
    }


    internal PartitionCollection(IDictionary<byte[], int> replacementsByText, IEnumerable<byte> utf8Bytes)
    {
        _replacementsByText = replacementsByText;
        _partitions = this.Load(utf8Bytes);
    }

    internal IEnumerable<int> Encode()
    {
        // Merge partitions based on rank
        while (this.HasMergeablePartitions)
        {
            // Since HasMergeablePartitions is true, we know
            // that the value of the lowest ranked pair is not null
            this.MergeWithNext(this.LowestRankedPartition!);
        }

        // Get the tokens for each remaining partition
        return _partitions.Select(p => p.Encode());
    }

    private void MergeWithNext(Partition front)
    {
        front.Value = front.Value.Concat(front.Next?.Value ?? Array.Empty<byte>());
        front.Next = front.Next?.Next;
        _partitions = this.Load(_partitions.First());
    }

    private List<Partition> Load(Partition first)
    {
        // Rebuild the list of partitions from
        // the first node by following the linked list
        var results = new List<Partition>();
        var current = first;
        while (current is not null)
        {
            results.Add(current);
            current = current.Next;
        }
        return results;
    }

    private List<Partition> Load(IEnumerable<byte> utf8Bytes)
    {
        // Break down the utf8 bytes into partitions
        var results = new List<Partition>();
        Partition? previous = null;
        for (var i = 0; i < utf8Bytes.Count(); i++)
        {
            var value = utf8Bytes.Skip(i).Take(1);
            var current = new Partition(_replacementsByText, value, null);
            results.Add(current);
            if (previous != null)
                previous.Next = current;
            previous = current;
        }
        return results;
    }

}
