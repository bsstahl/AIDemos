using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Extensions;
using Beary.Embeddings.LocalServer.Extensions;
using Beary.Search.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Image;

internal class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .UseLocalServerEmbeddingsModel()
            .UseBearySearch()
            .UseBearyReadRepository()
            .UseAzureAIEmbeddingsReadRepo()
            .UseAzureAIContentReadRepo()
            .BuildServiceProvider();

        var engine = services.GetRequiredService<Beary.ImageSearch.Engine>();
        await engine.Execute();
    }
}
