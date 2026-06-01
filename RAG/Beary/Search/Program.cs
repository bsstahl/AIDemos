using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Qdrant.Extensions;
using Beary.Documents.Extensions;
using Beary.Embeddings.LocalServer.Extensions;
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
            .UseBearyDocuments();

        var provider = config["BearyDb:Provider"];
        if (string.Equals(provider, "Qdrant", StringComparison.OrdinalIgnoreCase))
            services.UseQdrantBearyDb();
        else
            services.UseAzureAIBearyDb();

        var serviceProvider = services.BuildServiceProvider();

        var searchKey = args.FirstOrDefault() ?? throw new ArgumentException("SearchKey");

        var program = serviceProvider.GetRequiredService<Beary.Documents.Search>();
        var results = await program.GetRelevantArticles(searchKey, 10000);

        results.ToList().ForEach(r => Console.WriteLine(r.Title));
    }
}
