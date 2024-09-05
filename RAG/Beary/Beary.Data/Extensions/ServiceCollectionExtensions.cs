using Beary.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBearyWriteRepository(this IServiceCollection services)
    {
        return services
            .AddSingleton<IWriteContent, WriteRepository>();
    }

    public static IServiceCollection UseBearyReadRepository(this IServiceCollection services)
    {
        return services
            .AddSingleton<IReadContent, ReadRepository>();
    }
}
