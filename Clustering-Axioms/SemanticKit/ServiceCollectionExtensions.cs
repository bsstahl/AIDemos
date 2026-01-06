using Microsoft.Extensions.DependencyInjection;

namespace SemanticKit;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddLocalEmbeddingClient(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<IGetTextEmbeddings, TextEmbedder>();
        serviceCollection.AddHttpClient();
		return serviceCollection;
	}

    public static IServiceCollection AddSemanticKitChatClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IGetChatCompletions, ChatClient>();
        serviceCollection.AddHttpClient();
        return serviceCollection;
    }
}
