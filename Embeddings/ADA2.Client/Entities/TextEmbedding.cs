namespace ADA2.Client.Entities;

public class TextEmbedding
{
    /// <summary>
    /// An auto-incremented value used to identify the embedding within a collection
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// A tag that can be used as a shortcut identifier
    /// </summary>
    public string Tag { get; set; } = string.Empty;


    /// <summary>
    /// The text containing the semantic meaning
    /// </summary>
    public string EmbeddingText { get; set; } = string.Empty;


    /// <summary>
    /// The point that represents the location in vector space
    /// </summary>
    public float[]? EmbeddingValue { get; set; }

    /// <summary>
    /// The identity of the cluster this value was assigned to if clustering has been performed
    /// </summary>
    public int? ClusterId { get; set; }
}
