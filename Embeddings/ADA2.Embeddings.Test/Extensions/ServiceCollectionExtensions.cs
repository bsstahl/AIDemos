using ADA2.Client.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit.Abstractions;

namespace ADA2.Embeddings.Test.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAI(this IServiceCollection services, ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output).MinimumLevel.Verbose()
            .CreateLogger();

        var config = new ConfigurationBuilder()
            .AddJsonFile("settings.json", true)
            .AddUserSecrets<DistanceTests>()
            .Build();

        services
            .AddSingleton<EncodingEngine>()
            .AddSingleton<IConfiguration>(config)
            .AddLogging(l => l.AddSerilog(Log.Logger))
            .BuildServiceProvider();

        return services;
    }
}
