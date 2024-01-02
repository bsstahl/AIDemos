using ADA2.Client.Entities;

namespace ADA2.Client.Extensions;

public static class EncodingEngineExtensions
{
    // Used as a delegate to the Embedding functions in the GPT class
    public static async Task<Dictionary<string, float[]>> getEmbeddingsDelegate(this EncodingEngine encodingEngine, IEnumerable<string> values)
    {
        var dictionaryResult = new Dictionary<string, float[]>();
        var embeddings = await encodingEngine.EmbedAsync(values);

        var result = embeddings.IsError
            ? throw new InvalidOperationException($"Error executing language model")
            : embeddings!.Embeddings!;

        result.ToList().ForEach(e => dictionaryResult.Add(e.Key, e.Value));
        return dictionaryResult;
    }

}
