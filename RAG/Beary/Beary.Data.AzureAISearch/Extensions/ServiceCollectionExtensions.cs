using Beary.Application.Interfaces;
using Beary.Interfaces;
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

    public static IServiceCollection UseAzureAIContentReadRepo(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IReadContentSearchDocuments>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                var searchServiceName = config["SearchService:Name"];
                var apiKey = config["SearchService:ApiKey"];
                return new Content.ReadRepository(searchServiceName, apiKey);
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

    public static IServiceCollection UseAzureAIEmbeddingsReadRepo(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IReadEmbeddingsSearchDocuments>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                var searchServiceName = config["SearchService:Name"];
                var apiKey = config["SearchService:ApiKey"];
                return new Embeddings.ReadRepository(searchServiceName, apiKey);
            });
    }
}
