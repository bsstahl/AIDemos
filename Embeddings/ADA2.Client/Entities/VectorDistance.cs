namespace ADA2.Client.Entities;

public class VectorDistance
{
    public float Value { get; set; }

    public IEnumerable<float> SourcePoint { get; set; } = new List<float>();


    public TextEmbedding TargetEmbedding { get; set; } = new TextEmbedding();


    public override string ToString()
    {
        return $"\r\nThe distance to the '{TargetEmbedding.EmbeddingText}' vector is {Value}";
    }
}
