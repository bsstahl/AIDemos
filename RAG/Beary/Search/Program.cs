using Beary.Data.AzureAISearch.Extensions;
using Beary.Documents.Extensions;
using Beary.Embeddings.LocalServer.Extensions;
using Beary.Interfaces;
using Beary.ValueTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        var program = services.GetRequiredService<Beary.Documents.Search>();
        var results = await program.GetRelevantArticles(searchKey, 10000);

        results.ToList().ForEach(r => Console.WriteLine(r.Title));
    }
}
