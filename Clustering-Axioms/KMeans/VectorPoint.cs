namespace KMeans;

public sealed class VectorPoint(string id, float[] features)
{
    public string Id { get; } = id;
    public float[] Features { get; } = features;
}
