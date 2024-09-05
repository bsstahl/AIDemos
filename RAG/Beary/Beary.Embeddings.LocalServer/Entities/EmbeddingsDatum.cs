namespace Beary.Embeddings.LocalServer.Entities;

public class EmbeddingsDatum
{
    public string _object { get; set; }
    public float[] embedding { get; set; }
    public int index { get; set; }
}
