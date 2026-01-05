using AxiomVectorRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Cluster;

internal class Program
{
    const string _dbHostName = "localhost";

    static async Task Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddQdrantReadClient(_dbHostName)
            .AddSingleton<KMeans.Engine>()
            .AddSingleton<Engine>()
            .BuildServiceProvider();

        var axiomReadRepository = services.GetRequiredService<IGetAxiomVectors>();
        var axiomVectors = await axiomReadRepository.GetAxiomVectors();
        var points = axiomVectors.Select(v => new KMeans.VectorPoint(v.Id, v.Embedding)).ToArray();

        var kmeans = services.GetRequiredService<KMeans.Engine>();

        var k = 10;
        var fitAttempts = 1000;

        var clusteringResult = await kmeans.Fit(points, k, fitAttempts);
        var silhouetteScore = await kmeans.GetSilhouetteScore(points, clusteringResult);
        Console.WriteLine($"K: {k} - Silhouette Score: {silhouetteScore}");

        for (int i = 0; i < clusteringResult.Centroids.Count; i++)
        {
            var c = clusteringResult.Centroids[i];
            var a = axiomReadRepository.GetNearestAxiom(c);
            Console.WriteLine($"Cluster {i}: {a.Result.AxiomText}");
        }
        
    }
}
