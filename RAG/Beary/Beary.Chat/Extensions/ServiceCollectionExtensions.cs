using Microsoft.Extensions.DependencyInjection;

namespace Beary.Chat.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBearyChat(this IServiceCollection services)
    {
        return services
            .AddSingleton<Beary.Chat.OneShot>()
            .AddSingleton<Beary.Chat.MultiShot>();
    }
}
