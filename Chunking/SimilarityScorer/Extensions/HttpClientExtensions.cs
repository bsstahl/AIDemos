using System.Net.Http.Json;
using System.Text.Json;
using SimilarityScorer.Messages;

namespace SimilarityScorer.Extensions;

internal static class HttpClientExtensions
{
    internal static async Task<float[]> GetEmbeddingAsync(this HttpClient client, string model, string input)
    {
        var requestBody = new EmbeddingRequest(model, input);
        using var response = await client.PostAsJsonAsync("embeddings", requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"LM Studio embeddings request failed ({(int)response.StatusCode} {response.StatusCode}): {errorBody}");
        }

        var rawJson = await response.Content.ReadAsStringAsync();
        var payload = JsonSerializer.Deserialize<EmbeddingResponse>(rawJson);
        if (payload?.Data is null || payload.Data.Count == 0)
            throw new InvalidOperationException(
                $"LM Studio returned no embedding data. Raw response: {rawJson.Truncate(500)}");

        return payload.Data[0].Embedding;
    }
}
