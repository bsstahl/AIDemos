using Beary.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Data.AzureAISearch.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseAzureAIContentWriteRepo(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IWriteContentSearchDocuments>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                var searchServiceName = config["SearchService:Name"];
                var apiKey = config["SearchService:ApiKey"];
                return new Content.WriteRepository(searchServiceName, apiKey);
            });
    }

    public static IServiceCollection UseAzureAIEmbeddingsWriteRepo(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IWriteEmbeddingsSearchDocuments>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                var searchServiceName = config["SearchService:Name"];
                var apiKey = config["SearchService:ApiKey"];
                return new Embeddings.WriteRepository(searchServiceName, apiKey);
            });
    }
}
