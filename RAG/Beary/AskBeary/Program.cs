using Beary.Chat.AzureGpt.Extensions;
using Beary.Data.AzureAISearch.Extensions;
using Beary.Application.Extensions;
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
            .UseBearyChat()
            .UseAzureAIEmbeddingsReadRepo()
            .UseAzureAIContentReadRepo()
            .BuildServiceProvider();

        var engine = services.GetRequiredService<ChatEngine>();
        await engine.Execute();
    }
}
