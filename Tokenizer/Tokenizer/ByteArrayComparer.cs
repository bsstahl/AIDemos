namespace Tokenizer;

internal class ByteArrayComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[]? x, byte[]? y)
    {
        bool result = (x is null || y is null)
            ? ReferenceEquals(x, y)
            : true;

        result = result && x!.Length.Equals(y!.Length);
        for (int i = 0; i < x!.Length; i++)
            result = result && x![i] == y![i];

        return result;
    }

    public int GetHashCode(byte[] obj)
    {
        int hash = 17;
        foreach (byte element in obj)
            hash = hash * 31 + element;
        return hash;
    }
}
