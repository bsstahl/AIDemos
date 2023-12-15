using ADA2.Client.Entities;
using Azure.AI.OpenAI;

namespace ADA2.Client.Extensions;

public static class EmbeddingsOptionsExtensions
{
    public static EmbeddingsOptions AsEmbeddingsOptions(this ModelConfig modelConfig, IEnumerable<string> values, string? user = null)
    {
        return new EmbeddingsOptions(modelConfig.ModelName, values)
        {
            User = user
        };
    }
}