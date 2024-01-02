using ADA2.Client.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ADA2.Embeddings.Test.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAI(this IServiceCollection services, ILogger logger)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("settings.json", true)
            .AddUserSecrets<DistanceTests>()
            .Build();

        services
            .AddSingleton<EncodingEngine>()
            .AddSingleton<IConfiguration>(config)
            .AddScoped<EmbeddingCollection>()
            .AddLogging(l => l.AddSerilog(logger))
            .BuildServiceProvider();

        return services;
    }
}
