using System.Text.Json.Serialization;

namespace SimilarityScorer.Messages;

internal record EmbeddingRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("input")] string Input);
