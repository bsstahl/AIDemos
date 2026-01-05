using AxiomVectorRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Cluster;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddEngine(this IServiceCollection services, string? outputFolderPath = null)
        => services.AddSingleton<Engine>(s => new Engine(
            s.GetRequiredService<IGetAxiomVectors>(),
            s.GetRequiredService<KMeans.IClusterVectors>(),
            outputFolderPath));
}
