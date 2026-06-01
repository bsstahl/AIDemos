using Beary.Chat.AzureGpt.Extensions;
using Beary.Chat.Extensions;
using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Qdrant.Extensions;
using Beary.Documents.Extensions;
using Beary.Embeddings.LocalServer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AskBeary;

internal class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<ChatEngine>()
            .AddHttpClient()
            .UseAzureGptChatClient()
            .UseLocalServerEmbeddingsModel()
            .UseBearyDocuments()
            .UseBearyChat();

        var provider = config["BearyDb:Provider"];
        if (string.Equals(provider, "Qdrant", StringComparison.OrdinalIgnoreCase))
            services.UseQdrantBearyDb();
        else
            services.UseAzureAIBearyDb();

        var serviceProvider = services.BuildServiceProvider();

        var engine = serviceProvider.GetRequiredService<ChatEngine>();
        await engine.Execute();
    }
}
