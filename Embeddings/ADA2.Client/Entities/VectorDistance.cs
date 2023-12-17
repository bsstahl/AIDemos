namespace ADA2.Client.Entities;

public class VectorDistance
{
    public float Value { get; set; }

    public float[] SourcePoint { get; set; } = Array.Empty<float>();


    public TextEmbedding TargetEmbedding { get; set; } = new TextEmbedding();


    public override string ToString()
    {
        return $"\r\nThe distance to '{TargetEmbedding.EmbeddingText}' is {Value}";
    }
}
