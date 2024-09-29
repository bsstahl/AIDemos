using Microsoft.Extensions.DependencyInjection;

namespace Beary.Articles.FileSystem.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBearyFileSystemRepository(this IServiceCollection services)
    {
        return services.AddSingleton<Repository>();
    }
}
