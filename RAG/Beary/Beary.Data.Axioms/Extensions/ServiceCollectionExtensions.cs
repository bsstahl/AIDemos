using Beary.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Data.Axioms.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseAxiomRepository(this IServiceCollection services)
    {
        return services.AddSingleton<IReadEmbeddingsSearchDocuments, ReadRepository>();
    }
}
