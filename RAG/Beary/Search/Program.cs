using Beary.Data.AzureAISearch.Extensions;
using Beary.Embeddings.LocalServer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beary.Application.Extensions;

namespace Search;

internal class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<Program>()
            .UseLocalServerEmbeddingsModel()
            .UseAzureAIEmbeddingsReadRepo()
            .UseAzureAIContentReadRepo()
            .UseBearyDocuments()
            .BuildServiceProvider();

        var searchKey = args.FirstOrDefault() ?? throw new ArgumentException("SearchKey");

        var program = services.GetRequiredService<Beary.Application.Search>();
        var results = await program.GetRelevantArticles(searchKey, 10000);

        results.ToList().ForEach(r => Console.WriteLine(r.Title));
    }
}
