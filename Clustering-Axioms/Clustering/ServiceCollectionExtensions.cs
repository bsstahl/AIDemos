using AxiomVectorRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Clustering;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddEngine(this IServiceCollection services, string? outputFolderPath = null)
        => services.AddSingleton<Engine>(s => new Engine(
            s.GetRequiredService<IGetAxiomVectors>(),
            s.GetRequiredService<KMeans.IClusterVectors>(),
            s.GetRequiredService<SemanticKit.IGetChatCompletions>(),
            outputFolderPath));
}
