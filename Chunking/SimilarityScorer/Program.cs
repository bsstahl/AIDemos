using System.Text.Json;
using System.Text.Json.Serialization;
using SimilarityScorer.Extensions;

// -----------------------------------------------------------------------
// Similarity Chunker
//
// Fetches text embeddings for a fixed set of sentences from a local
// LM Studio server (OpenAI-compatible /v1/embeddings endpoint) running the
// "text-embedding-nomic-embed-text-v2" model, then computes the pairwise
// cosine similarity between every sentence combination.
// 
// Once the similarity matrix is computed, a dynamic programming algorithm
// is used to find the optimal segmentation of the sentences into cohesive
// chunks, where each chunk is a contiguous sequence of sentences that are
// more similar to each other than to sentences outside the chunk. The scoring
// function for a chunk is defined as the average pairwise similarity within
// the chunk minus a penalty proportional to the chunk size, which discourages
// overly large chunks unless the sentences are genuinely cohesive.
//
// Results are written to JSON files in the `OutputDir` for later study:
//   embeddings.json         - raw embedding vectors per sentence
//   similarity-matrix.json  - full grid of cosine similarity values
// -----------------------------------------------------------------------

var sentences = new[]
{
    "The clarity-first, object-oriented implementation of a Tokenizer is written in C#, my language of choice.",
    "I suspect it will be easy to have it translated into nearly any other programming language if that will make it easier for you to understand.",
    "The goal of this implementation isn't speed, it's transparency.",
    "You can step through Encode and Decode to see exactly what's happening.",
    "The code is available on GitHub."
};
var n = sentences.Length;

var outputDir = AppContext.BaseDirectory;
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

var embeddings = await sentences.GetEmbeddings();
await embeddings.WriteToDisk(outputDir, sentences, jsonOptions);

// ---- Compute pairwise cosine similarity grid ----
var similarityMatrix = new double[sentences.Length][];
var pairs = similarityMatrix.Populate(sentences, embeddings);

similarityMatrix.WriteToConsole();
await similarityMatrix.WriteToDisk(outputDir, sentences, pairs, jsonOptions);


// ---- Print chunk score table ----
Console.WriteLine();
Console.WriteLine("Chunk Score Table:");
for (var ci = 1; ci <= n; ci++)
{
    for (var cj = ci; cj <= n; cj++)
    {
        var cs = similarityMatrix.ScoreChunk(ci, cj);
        Console.WriteLine($"  Chunk [{ci}..{cj}] Score = {cs:0.0000}");
    }
}

// ---- Dynamic programming segmentation ----
// dp[i] = best total score achievable starting at sentence i (1-based).
// dp[N+1] = 0 is the base case (no sentences left).
var dp = new double[n + 2];
var nextSentence = new int[n + 2];
dp[n + 1] = 0.0;

for (var i = n; i >= 1; i--)
{
    var bestScore = double.NegativeInfinity;
    for (var j = i; j <= n; j++)
    {
        var score = similarityMatrix.ScoreChunk(i, j);
        var candidate = score + dp[j + 1];
        if (candidate > bestScore)
        {
            bestScore = candidate;
            nextSentence[i] = j;
        }
    }
    dp[i] = bestScore;
}

// ---- Print DP memoization table ----
Console.WriteLine();
Console.WriteLine("DP Memoization Table:");
for (var i = n + 1; i >= 1; i--)
    Console.WriteLine($"  DP[{i}] = {dp[i]:0.0000}");

// ---- Print next[] choices ----
Console.WriteLine();
Console.WriteLine("Optimal next[] values:");
for (var i = 1; i <= n; i++)
    Console.WriteLine($"  next[{i}] = {nextSentence[i]}");

// ---- Reconstruct and print optimal chunking ----
Console.WriteLine();
Console.WriteLine("Optimal Chunks:");
var start = 1;
while (start <= n)
{
    var end = nextSentence[start];
    Console.WriteLine($"  [{start}..{end}]");
    start = end + 1;
}

return;

