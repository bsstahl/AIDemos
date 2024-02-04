namespace Tokenizer.Extensions;

internal static class ByteExtensions
{
    internal static IEnumerable<(int Start, int Rank)> AsPartitions(this IEnumerable<byte> bytes)
    {
        return Enumerable.Range(0, bytes.Count() + 1)
            .Select(i => (i, int.MaxValue));
    }
}
