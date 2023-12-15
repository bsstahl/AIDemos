using ADA2.Client.Entities;
using ADA2.Client.Extensions;
using Microsoft.Extensions.Logging;

namespace ADA2.Embeddings.Test.Extensions;

internal static class EncodingEngineExtensions
{
    internal static async Task<IEnumerable<VectorDistance>> GetDistances(this EncodingEngine encodingEngine, ILogger logger, EmbeddingCollection dictionary, string testStatement)
    {
        // Fetch embeddings for all of the statements from the OpenAI service
        await dictionary.PopulateEmbeddings(encodingEngine.getEmbeddingsDelegate, TimeSpan.FromSeconds(0));

        // Fetch the embedding for the test statement from the OpenAI service
        var (IsError, Embedding) = await encodingEngine.EmbedAsync(new string[] { testStatement });
        var testEmbeddingValue = IsError
            ? throw new InvalidOperationException("Unable to perform embedding")
            : Embedding!.Single().Value;

        // Calculate the vector distances between each statement and the test statement
        return dictionary.GetNearestNeighbors(testEmbeddingValue, float.MinValue);
    }
}
