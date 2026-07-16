using System.Text.Json;
using SimilarityScorer.Messages;

namespace SimilarityScorer.Extensions;

internal static class DoubleExtensions
{
    public static void WriteToConsole(this Double[][] similarityMatrix)
    {
        // ---- Print a quick human-readable grid to the console ----
        var n = similarityMatrix.Length;
        if (!similarityMatrix.All(row => row.Length == n))
        {
            Console.WriteLine("Error: similarityMatrix is not square.");
            throw new InvalidOperationException("similarityMatrix is not square.");
        }

        Console.WriteLine();
        Console.WriteLine("Cosine Similarity Grid:");
        Console.Write("        ");
        for (var j = 0; j < n; j++) Console.Write($"S{j + 1}      ");
        Console.WriteLine();
        for (var i = 0; i < n; i++)
        {
            Console.Write($"S{i + 1}      ");
            for (var j = 0; j < n; j++)
            {
                Console.Write($"{similarityMatrix[i][j]:0.0000} ");
            }
            Console.WriteLine();
        }
    }

    // -----------------------------------------------------------------------
    // Scoring helper (uses cosine similarity; captures similarityMatrix / n)
    // -----------------------------------------------------------------------

    // ScoreChunk uses 1-based sentence indices (i..j inclusive).
    // It averages all pairwise similarities within the chunk, then subtracts
    // a penalty proportional to chunk size so that large chunks are only
    // preferred when the sentences are genuinely cohesive.
    //
    //   score(i,j) = avgSimilarity(i..j) - 0.1 * (chunkSize - 1)
    //
    // A single-sentence chunk has no pairs → avgSimilarity = 0 → score = 0.
    internal static double ScoreChunk(this Double[][] similarityMatrix, int i, int j)
    {
        var chunkSize = j - i + 1;
        var values = new List<double>();
        for (var a = i - 1; a <= j - 1; a++)
            for (var b = a + 1; b <= j - 1; b++)
                values.Add(similarityMatrix[a][b]);

        var avg = values.Count > 0 ? values.Average() : 0.0;
        return avg - 0.1 * (chunkSize - 1);
    }

    internal static async Task WriteToDisk(this List<Single[]> embeddings, String outputDir, String[] sentences, JsonSerializerOptions jsonOptions)
    {
        // ---- Write raw embeddings to disk ----
        var embeddingRecords = sentences
            .Select((s, i) => new SentenceEmbedding(i + 1, s, embeddings[i]))
            .ToList();

        var embeddingsPath = Path.Combine(outputDir, "embeddings.json");
        await File.WriteAllTextAsync(embeddingsPath, JsonSerializer.Serialize(embeddingRecords, jsonOptions));
        Console.WriteLine($"Wrote embeddings to {embeddingsPath}");
    }

    internal static List<SimilarityPair> Populate(this Double[][] similarityMatrix, String[] sentences, List<Single[]> embeddings)
    {
        var n = similarityMatrix.Length;
        var pairs = new List<SimilarityPair>();
        for (var i = 0; i < n; i++)
        {
            similarityMatrix[i] = new double[n];

            for (var j = 0; j < n; j++)
            {
                var similarity = embeddings[i].CosineSimilarity(embeddings[j]);

                similarityMatrix[i][j] = similarity;

                if (j > i)
                {
                    pairs.Add(new SimilarityPair(
                        SentenceAIndex: i + 1,
                        SentenceA: sentences[i],
                        SentenceBIndex: j + 1,
                        SentenceB: sentences[j],
                        CosineSimilarity: similarity));
                }
            }
        }

        return pairs;
    }

    internal static async Task WriteToDisk(this Double[][] similarityMatrix, String outputDir, String[] sentences, List<SimilarityPair> pairs, JsonSerializerOptions jsonOptions)
    {
        var result = new SimilarityResult(
            Sentences: sentences.Select((s, i) => new IndexedSentence(i + 1, s)).ToList(),
            CosineSimilarityMatrix: similarityMatrix,
            Pairs: pairs.OrderByDescending(p => p.CosineSimilarity).ToList()
        );

        var matrixPath = Path.Combine(outputDir, "similarity-matrix.json");
        await File.WriteAllTextAsync(matrixPath, JsonSerializer.Serialize(result, jsonOptions));
        Console.WriteLine($"Wrote similarity matrix to {matrixPath}");
    }
}
