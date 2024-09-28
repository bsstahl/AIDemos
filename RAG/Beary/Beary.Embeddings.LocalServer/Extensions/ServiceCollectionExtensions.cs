using Beary.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Embeddings.LocalServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseLocalServerEmbeddingsModel(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddHttpClient()
            .AddSingleton<IGetEmbeddings, LocalServer.Client>();
    }
}
