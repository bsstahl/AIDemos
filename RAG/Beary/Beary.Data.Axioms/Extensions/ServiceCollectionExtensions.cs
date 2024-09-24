using Microsoft.Extensions.DependencyInjection;
using Beary.Data.Interfaces;

namespace Beary.Data.Axioms.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseAxiomRepository(this IServiceCollection services)
    {
        return services.AddSingleton<IReadEmbeddingsSearchDocuments, ReadRepository>();
    }
}
