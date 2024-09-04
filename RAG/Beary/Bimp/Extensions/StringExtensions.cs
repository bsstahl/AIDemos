using Beary.Entities;
using Beary.ValueTypes;
using System.Text;
using System.Text.Json;

namespace Bimp.Extensions;

internal static class StringExtensions
{
    const string modelName = "nomic-ai/nomic-embed-text-v1.5-GGUF";
    const string url = "http://localhost:1234/v1/embeddings";

    private static readonly HttpClient client = new HttpClient();

    internal async static Task<IEnumerable<ContentChunk>> GetEmbeddings(this IEnumerable<string> inputText, string articleId)
    {
        var payload = new
        {
            model = modelName,
            input = inputText.ToArray()
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(responseBody);
        return embeddingResponse?.data
            .Select(d => new ContentChunk($"{articleId}_{d.index}", 
                d.index, payload.input[d.index], d.embedding)) ?? [];
    }
}


public class EmbeddingResult(int Index, string Text, IEnumerable<float> Embedding)
{ }

internal class EmbeddingResponse
{
    public string _object { get; set; }
    public Datum[] data { get; set; }
    public string model { get; set; }
    public Usage usage { get; set; }
}

public class Usage
{
    public int prompt_tokens { get; set; }
    public int total_tokens { get; set; }
}

public class Datum
{
    public string _object { get; set; }
    public float[] embedding { get; set; }
    public int index { get; set; }
}

