using AxiomVectorRepository;
using KMeans;
using Microsoft.Extensions.DependencyInjection;
using SemanticKit;

namespace Clustering;

internal class Program
{
    const string _dbHostName = "localhost";

    const int _fitAttemptMin = 100;
    const Single _toleranceMin = 1e-3f;

    const int _fitAttemptsStandard = 5000;
    const Single _toleranceStandard = 1e-5f;

    const int _fitAttemptsMax = 100000;
    const Single _toleranceMax = 1e-10f;

    static async Task Main(string[] args)
    {
        // string? outputPath = null;
        string? outputPath = Path.Combine(Path.GetTempPath(), "Clusters");

        var services = new ServiceCollection()
            .AddQdrantReadClient(_dbHostName)          // The vector data source
            .AddKMeansEngine(Engine.ShowPartialResult) // The clustering algorithm
            .AddSemanticKitChatClient()                // The LLM chat client for labeling
            .AddEngine(outputPath)                     // The process we are running
            .BuildServiceProvider();

        var engine = services.GetRequiredService<Engine>();

        // var labels = await engine.CreateLabels(2, 5, _fitAttemptsStandard, _toleranceStandard);
        // var labels = await engine.CreateLabels(8, 12, _fitAttemptsStandard, _toleranceStandard);
        var labels = await engine.CreateLabels(20, 30, _fitAttemptsStandard, _toleranceStandard);
        // var labels = await engine.CreateLabels(90, 110, _fitAttemptsStandard, _toleranceStandard);

        WriteLabelingResults(labels, $"Clusters\\ClusteringResults_{DateTime.UtcNow.Ticks}.yml");

        //await engine.FindBestClustering(2, 5, _fitAttemptsStandard, _toleranceStandard);
        //await engine.FindBestClustering(8, 12, _fitAttemptsStandard, _toleranceStandard);
        //await engine.FindBestClustering(20, 30, _fitAttemptsStandard, _toleranceStandard);
        //await engine.FindBestClustering(90, 110, _fitAttemptsStandard, _toleranceStandard);

        // await engine.GetKClusters(4, _fitAttemptsMax, _toleranceMax);
    }

    public static void WriteLabelingResults(IEnumerable<Cluster> clusters, string pathInTempFolder)
    {
        var filePath = Path.Combine(Path.GetTempPath(), pathInTempFolder);
        var data = clusters.Serialize();
        File.WriteAllText(filePath, data);
    }
}
