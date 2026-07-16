using System.Text.Json.Serialization;

namespace SimilarityScorer.Messages;

internal record EmbeddingData(
    [property: JsonPropertyName("embedding")] float[] Embedding,
    [property: JsonPropertyName("index")] int Index);
