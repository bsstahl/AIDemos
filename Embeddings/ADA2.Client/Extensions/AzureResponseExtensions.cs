using Azure.AI.OpenAI;
using Azure;

namespace ADA2.Client.Extensions;

public static class AzureResponseExtensions
{
    public static (bool IsError, IDictionary<string, float[]>? Embeddings) AsEmbeddingsModelResponse(this Response<Embeddings> response, EmbeddingsOptions embeddingOptions)
    {
        var isError = response.GetRawResponse().IsError;
        IDictionary<string, float[]>? embeddings = null;
        var inputData = embeddingOptions.Input.ToArray();
        var outputData = response.Value.Data.ToArray();

        if (!isError)
        {
            embeddings = new Dictionary<string, float[]>();
            for (int i = 0; i < inputData.Length; i++)
                embeddings.Add(inputData[i], outputData[i].Embedding.ToArray());
        }

        return (isError, embeddings);
    }
}
