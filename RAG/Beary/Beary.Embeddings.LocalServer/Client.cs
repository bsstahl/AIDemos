using System;
using System.Text.Json;
using System.Text;
using Beary.Entities;
using Beary.Interfaces;
using Beary.Embeddings.LocalServer.Entities;

namespace Beary.Embeddings.LocalServer;

public class Client(IHttpClientFactory httpClientFactory) : IGetEmbeddings
{
    // TODO: Move to config
    const string modelName = "nomic-ai/nomic-embed-text-v1.5-GGUF";
    const string url = "http://localhost:1234/v1/embeddings";

    public async Task<ContentChunk?> GetEmbedding(string inputText, string baseId)
    {
        var embeddings = await GetEmbeddings(new string[] { inputText }, baseId).ConfigureAwait(false);
        return embeddings.FirstOrDefault();
    }

    public async Task<IEnumerable<ContentChunk>> GetEmbeddings(IEnumerable<string> inputText, string baseId)
    {
        var payload = new
        {
            model = modelName,
            input = inputText.ToArray()
        };

        var jsonPayload = JsonSerializer.Serialize(payload);

        using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        using var httpClient = httpClientFactory.CreateClient();

        var response = await httpClient.PostAsync(new Uri(url), content).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(responseBody);
        var result = embeddingResponse?.data
            .Select(d => new ContentChunk($"{baseId}_{d.index}",
                d.index, payload.input[d.index], d.embedding)) ?? [];
        
        return result;
    }
}

