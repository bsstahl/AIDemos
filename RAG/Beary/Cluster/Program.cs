using Accord.MachineLearning;
using Beary.Chat.AzureGpt.Extensions;
using Beary.Chat.Interfaces;
using Beary.Data.Axioms.Extensions;
using Beary.Data.Interfaces;
using Beary.Embeddings.LocalServer.Extensions;
using Beary.ValueTypes;
using Cluster.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cluster;

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
            .AddHttpClient()
            .UseLocalServerEmbeddingsModel()
            .UseAxiomRepository()
            .UseAzureGptChatClient()
            .BuildServiceProvider();

        var program = services.GetRequiredService<Program>();
        await program.GetParagraphClusters();
    }

    private readonly IReadEmbeddingsSearchDocuments _embeddingsReadRepo;
    private readonly ICreateChatCompletions _chatClient;

    public Program(IReadEmbeddingsSearchDocuments embeddingsReadRepo, ICreateChatCompletions chatClient)
    {
        _embeddingsReadRepo = embeddingsReadRepo;
        _chatClient = chatClient;
    }

    internal async Task GetParagraphClusters()
    {
        const int k = 16;

        // Fetch all embeddings from the Azure AI Search service
        var embeddings = await _embeddingsReadRepo.GetAllEmbeddings();
        var clusters = (await GetClusters(embeddings, k)).ToList();

        foreach (var cluster in clusters)
            await cluster.Identify(_chatClient);

        var path = $"c:\\s\\Temp\\BlogClusters\\{Path.GetRandomFileName()}.md";
        File.WriteAllText(path, string.Join("\r\n", clusters));
        Console.WriteLine($"Output written to: {path}");
    }

    private async Task<IEnumerable<DocumentCluster>> GetClusters(IEnumerable<Beary.Entities.SearchResult> embeddings, int k)
    {
        var results = new List<DocumentCluster>();

        // Calculate the Clusters
        var kmeans = new KMeans(k);
        var clusterCollection = kmeans.Learn(embeddings.AsEmbeddingsArray());

        foreach (var cluster in clusterCollection)
        {
            int clusterActualCount = Convert.ToInt32(Math.Floor(cluster.Proportion * Convert.ToDouble(embeddings.Count())));
            var elements = await _embeddingsReadRepo.GetNearestNeighbors(cluster.CentroidVector(), ResultCount.From(clusterActualCount));
            var centroidVector = Vector.From(cluster.Centroid.Select(f => Convert.ToSingle(f)));
            var documentCluster = new DocumentCluster(cluster.Index, null, centroidVector, elements);
            results.Add(documentCluster);
        }

        return results;
    }

}
