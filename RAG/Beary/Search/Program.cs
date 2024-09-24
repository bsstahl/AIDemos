using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Extensions;
using Beary.Embeddings.LocalServer.Extensions;
using Beary.Interfaces;
using Beary.Search.Extensions;
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
            .UseBearySearch()
            .UseBearyReadRepository()
            .AddHttpClient()
            .BuildServiceProvider();

        var searchKey = args.FirstOrDefault() ?? throw new ArgumentException("SearchKey");

        var program = services.GetRequiredService<Program>();
        var results = await program.GetArticles(searchKey);

        results.ToList().ForEach(r => Console.WriteLine(r.Title));
    }

    private readonly IFindDocuments _searchClient;

    public Program(IFindDocuments searchClient)
    {
        _searchClient = searchClient;
    }

    public async Task<IEnumerable<Beary.Entities.Article>> GetArticles(string searchKey)
    {
        var maxTokenCount = TokenCount.From(10000);
        return await _searchClient.GetRelevantArticles(searchKey, maxTokenCount);
    }
}
