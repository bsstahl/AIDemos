using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

// ── Configuration ────────────────────────────────────────────────────────────
// LM Studio must be running with a model loaded and the local server enabled.
const string lmStudioUrl  = "http://localhost:1234/v1/embeddings";
const string modelName    = "text-embedding-mxbai-embed-large-v1";

// ── Phrases ──────────────────────────────────────────────────────────────────
const string targetPhrase = "How can I stop my laptop battery from draining overnight?";

string[] comparisonPhrases =
[
    "Reduce background apps and network activity during standby.",
    "Prevent a notebook from losing charge while it sleeps.",
    "Replace a worn battery that no longer holds a charge.",
    "Drain the laptop battery completely overnight before recalibrating it.",
];

// ── Embeddings ───────────────────────────────────────────────────────────────
Console.WriteLine($"Target: \"{targetPhrase}\"\n");

using var http = new HttpClient();

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull
};

float[] targetEmbedding = await GetEmbeddingAsync(http, jsonOptions, lmStudioUrl, modelName, targetPhrase);

// ── Comparison ───────────────────────────────────────────────────────────────
var results = new List<(string Phrase, float Distance)>();

foreach (var phrase in comparisonPhrases)
{
    float[] embedding = await GetEmbeddingAsync(http, jsonOptions, lmStudioUrl, modelName, phrase);
    float distance    = CosineDistance(targetEmbedding, embedding);
    results.Add((phrase, distance));
}

// ── Output ───────────────────────────────────────────────────────────────────
Console.WriteLine($"{"Cosine Distance",-18} Phrase");
Console.WriteLine(new string('-', 80));

foreach (var (phrase, distance) in results.OrderBy(r => r.Distance))
    Console.WriteLine($"{distance,-18:F6} {phrase}");

// ── Helpers ──────────────────────────────────────────────────────────────────
static async Task<float[]> GetEmbeddingAsync(
    HttpClient http, JsonSerializerOptions jsonOptions, string url, string model, string text)
{
    var request  = new EmbeddingRequest { Model = model, Input = text };
    var response = await http.PostAsJsonAsync(url, request, jsonOptions);

    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>()
        ?? throw new InvalidOperationException("Empty response from embedding service.");

    return result.Data[0].Embedding;
}

static float CosineDistance(float[] a, float[] b)
{
    if (a.Length != b.Length)
        throw new ArgumentException("Vectors must have the same number of dimensions.");

    float dot  = 0f, magA = 0f, magB = 0f;

    for (int i = 0; i < a.Length; i++)
    {
        dot  += a[i] * b[i];
        magA += a[i] * a[i];
        magB += b[i] * b[i];
    }

    float similarity = dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
    return 1f - similarity;
}

// ── DTOs ─────────────────────────────────────────────────────────────────────
class EmbeddingRequest
{
    [JsonPropertyName("model")] public string Model { get; set; } = "";
    [JsonPropertyName("input")] public string Input { get; set; } = "";
}

class EmbeddingResponse
{
    [JsonPropertyName("data")] public List<EmbeddingData> Data { get; set; } = [];
}

class EmbeddingData
{
    [JsonPropertyName("embedding")] public float[] Embedding { get; set; } = [];
}

