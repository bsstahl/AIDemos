namespace SimilarityScorer.Extensions;

internal static class StringExtensions
{
    internal static string Truncate(this string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength] + "...";


    internal static async Task<List<Single[]>> GetEmbeddings(this String[] sentences)
    {
        const string LmStudioBaseUrl = "http://localhost:1234/v1/";
        const string EmbeddingModel = "text-embedding-nomic-embed-text-v2";

        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri(LmStudioBaseUrl),
            Timeout = TimeSpan.FromMinutes(2)
        };

        Console.WriteLine($"Requesting embeddings from {LmStudioBaseUrl} using model '{EmbeddingModel}'...");

        var embeddings = new List<float[]>();
        for (var i = 0; i < sentences.Length; i++)
        {
            Console.WriteLine($"  [{i + 1}/{sentences.Length}] Embedding: \"{sentences[i].Truncate(60)}\"");
            var embedding = await httpClient.GetEmbeddingAsync(EmbeddingModel, sentences[i]);
            embeddings.Add(embedding);
        }

        return embeddings;
    }
}
