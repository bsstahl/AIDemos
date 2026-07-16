using Beary.Data.Db.Repositories;
using Beary.Documents.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Data.Db.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBearyDataStoreAbstraction(this IServiceCollection services)
    {
        return services
            .AddSingleton<IReadContentSearchDocuments, ContentReadRepository>()
            .AddSingleton<IWriteContentSearchDocuments, ContentWriteRepository>()
            .AddSingleton<IReadEmbeddingsSearchDocuments, EmbeddingsReadRepository>()
            .AddSingleton<IWriteEmbeddingsSearchDocuments, EmbeddingsWriteRepository>();
    }
}
