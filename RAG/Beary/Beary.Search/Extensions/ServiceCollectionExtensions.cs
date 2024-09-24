using Beary.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Search.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBearySearch(this IServiceCollection services)
    {
        services.AddSingleton<IFindDocuments, Client>();
        return services;
    }
}
