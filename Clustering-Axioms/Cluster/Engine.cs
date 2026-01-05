using System.Text;
using AxiomVectorRepository;
using KMeans;

namespace Cluster;

internal class Engine
{
    private readonly IGetAxiomVectors _axiomReadRepository;
    private readonly IClusterVectors _kMeansEngine;
    private readonly string? _writeToFolder;

    public Engine(IGetAxiomVectors axiomReadRepository, KMeans.IClusterVectors kMeansEngine, string? writeToFolder = null)
    {
        _axiomReadRepository = axiomReadRepository;
        _kMeansEngine = kMeansEngine;
        _writeToFolder = writeToFolder;
    }

    public async Task FindBestClustering(int minK, int maxK, int fitAttempts, Single tolerance)
    {
        var axiomVectors = await _axiomReadRepository.GetAxiomVectors();
        var points = axiomVectors.Select(v => new KMeans.VectorPoint(v.Id, v.Embedding)).ToArray();

        var clusteringResult = await _kMeansEngine.Fit(points, minK, maxK, fitAttempts, tolerance);
        var silhouetteScore = await _kMeansEngine.GetSilhouetteScore(points, clusteringResult);
        var bestK = clusteringResult.Centroids.Count;

        this.OutputResults("Best Fit", clusteringResult, silhouetteScore, bestK);
    }

    public async Task GetKClusters(int k, int fitAttempts, Single tolerance)
    {
        var axiomVectors = await _axiomReadRepository.GetAxiomVectors();
        var points = axiomVectors.Select(v => new KMeans.VectorPoint(v.Id, v.Embedding)).ToArray();

        var clusteringResult = await _kMeansEngine.Fit(points, k, fitAttempts, tolerance);
        var silhouetteScore = await _kMeansEngine.GetSilhouetteScore(points, clusteringResult);
        var bestK = clusteringResult.Centroids.Count;

        this.OutputResults("K Clusters", clusteringResult, silhouetteScore, bestK);
    }

    private void OutputResults(string title, KMeansResult clusteringResult, Single silhouetteScore, Int32 bestK)
    {
        this.DisplayResults(title, clusteringResult, bestK, silhouetteScore);
        this.WriteResults(title, clusteringResult, bestK, silhouetteScore);
    }

    private void DisplayResults(String title, KMeansResult clusteringResult, Int32 bestK, Single silhouetteScore)
    {
        Console.WriteLine(this.CreateResults(title, clusteringResult, bestK, silhouetteScore));
    }

    private void WriteResults(String title, KMeansResult clusteringResult, Int32 bestK, Single silhouetteScore)
    {
        if (!string.IsNullOrWhiteSpace(_writeToFolder))
        {
            if (!Path.Exists(_writeToFolder))
                Directory.CreateDirectory(_writeToFolder);
            
            var filePath = Path.Combine(_writeToFolder, $"{title.Replace(" ", "_")}_K{bestK}_{DateTime.UtcNow.Ticks}.txt");
            var fileText = this.CreateResults(title, clusteringResult, bestK, silhouetteScore);
            File.WriteAllText(filePath, fileText);
            Console.WriteLine($"Results written to: {filePath}");
        }
    }

    internal static void ShowPartialResult(string resultText)
    {
        Console.WriteLine(resultText);
    }

    private string CreateResults(String title, KMeansResult clusteringResult, Int32 bestK, Single silhouetteScore)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"\r\n{title}: K={bestK} - Silhouette Score: {silhouetteScore}");
        for (int i = 0; i < clusteringResult.Centroids.Count; i++)
        {
            var c = clusteringResult.Centroids[i];
            var a = _axiomReadRepository.GetNearestAxiom(c);
            sb.AppendLine($"\r\nCluster {i}: {a.Result.AxiomText}");
        }
        return sb.ToString();
    }

}