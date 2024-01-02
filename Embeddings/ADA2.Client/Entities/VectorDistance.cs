namespace ADA2.Client.Entities;

public class VectorDistance
{
    public float Value { get; set; }

    public TextEmbedding SourceEmbedding { get; set; } = new TextEmbedding();

    public TextEmbedding TargetEmbedding { get; set; } = new TextEmbedding();


    public override string ToString()
    {
        var source = SourceEmbedding.Tag is not null && !SourceEmbedding.Tag.Equals(SourceEmbedding.EmbeddingText)
            ? $"{SourceEmbedding.EmbeddingText} [{SourceEmbedding.Tag}]"
            : SourceEmbedding.EmbeddingText;

        var target = TargetEmbedding.Tag is not null && !TargetEmbedding.Tag.Equals(TargetEmbedding.EmbeddingText)
            ? $"{TargetEmbedding.EmbeddingText} [{TargetEmbedding.Tag}]"
            : TargetEmbedding.EmbeddingText;

        return $"\r\nThe distance from '{source}' to '{target}' is {Value}";
    }
}
