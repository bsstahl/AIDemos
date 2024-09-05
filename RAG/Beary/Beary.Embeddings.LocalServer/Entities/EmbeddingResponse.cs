namespace Beary.Embeddings.LocalServer.Entities;

internal class EmbeddingResponse
{
    public string _object { get; set; }
    public EmbeddingsDatum[] data { get; set; }
    public string model { get; set; }
    public ModelUsage usage { get; set; }
}

