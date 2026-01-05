using Microsoft.Extensions.DependencyInjection;

namespace KMeans;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKMeansEngine(this IServiceCollection services, Action<string>? intermediateClusteringResultsDelegate = null)
        => services.AddSingleton<KMeans.IClusterVectors>(e => new KMeans.Engine(intermediateClusteringResultsDelegate));
}
