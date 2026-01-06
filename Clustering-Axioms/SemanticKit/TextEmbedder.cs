using System.Net.Http.Json;
using System.Text.Json;

namespace SemanticKit;

internal class TextEmbedder : IGetTextEmbeddings
{
	const string _endpointUrl = "http://localhost:1234/v1/embeddings";
    const string _modelName = "text-embedding-nomic-embed-text-v1.5";

    private readonly HttpClient _httpClient;

    public TextEmbedder(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<float[]>> GetEmbeddingsAsync(IEnumerable<string> dataToEmbed)
    {
        throw new NotImplementedException();
    }

    public async Task<float[]> GetEmbeddingAsync(string textToEmbed)
    {
        // Build request payload
        var payload = new
        {
            model = _modelName,
            input = textToEmbed
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl);
        //request.Headers.Authorization =
        //	string.IsNullOrWhiteSpace(apiKey) ? null :
        //	new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = JsonContent.Create(payload);
        // Console.WriteLine($"Request Payload: {JsonSerializer.Serialize(payload)}");

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // Parse JSON
        using var stream = await response.Content.ReadAsStreamAsync();
        var json = await JsonDocument.ParseAsync(stream);

        // Navigate: data[0].embedding
        var embeddingJson = json
            .RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding");

        // Convert to float[]
        var embedding = embeddingJson
            .EnumerateArray()
            .Select(x => x.GetSingle())
            .ToArray();

        return embedding;
    }
}
