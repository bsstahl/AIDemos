using Microsoft.Extensions.DependencyInjection;

namespace Beary.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBearyDocuments(this IServiceCollection services)
    {
        return services
            .AddSingleton<Import>()
            .AddSingleton<Search>();
    }

    public static IServiceCollection UseBearyChat(this IServiceCollection services)
    {
        return services
            .AddSingleton<Beary.Application.OneShot>()
            .AddSingleton<Beary.Application.MultiShot>();
    }
}
