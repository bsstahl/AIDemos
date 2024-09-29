using Microsoft.Extensions.DependencyInjection;

namespace Beary.Documents.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBearyDocuments(this IServiceCollection services)
    {
        return services
            .AddSingleton<Import>()
            .AddSingleton<Search>();
    }
}
