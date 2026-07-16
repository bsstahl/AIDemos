using System.Text.Json.Serialization;

namespace SimilarityScorer.Messages;

internal record EmbeddingResponse(
    [property: JsonPropertyName("data")] List<EmbeddingData> Data);
