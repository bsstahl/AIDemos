using System.Text;
using AxiomVectorRepository;
using KMeans;
using SemanticKit;

namespace Clustering;

internal class Engine
{
    const float _percentOfAxiomsToUseForLabeling = 0.08f;

    private readonly IGetAxiomVectors _axiomReadRepository;
    private readonly IClusterVectors _kMeansEngine;
    private readonly IGetChatCompletions _chatClient;
    private readonly string? _writeToFolder;

    public Engine(IGetAxiomVectors axiomReadRepository, IClusterVectors kMeansEngine, IGetChatCompletions chatClient, string? writeToFolder = null)
    {
        _axiomReadRepository = axiomReadRepository;
        _kMeansEngine = kMeansEngine;
        _chatClient = chatClient;
        _writeToFolder = writeToFolder;
    }

    public async Task<IEnumerable<Cluster>> CreateLabels(int minK, int maxK, int fitAttempts, Single tolerance)
    {
        var axiomVectors = await _axiomReadRepository.GetAxiomVectors();
        var points = axiomVectors.Select(v => new KMeans.VectorPoint(v.Id, v.Embedding)).ToArray();

        var clusteringResult = await _kMeansEngine.Fit(points, minK, maxK, fitAttempts, tolerance);
        var silhouetteScore = await _kMeansEngine.GetSilhouetteScore(points, clusteringResult);
        var bestK = clusteringResult.Centroids.Count;

        var clusterIds = clusteringResult.AssignmentsById.Values.Distinct().ToList();

        var results = new List<Cluster>();
        foreach (var clusterId in clusterIds)
        {
            var axiomsIdsForCluster = clusteringResult.AssignmentsById
                .Where(a => a.Value == clusterId);

            var axiomsInCluster = await _axiomReadRepository
                .GetAxiomVectorsByIds(axiomsIdsForCluster.Select(a => a.Key));

            var clusterCentroid = clusteringResult.Centroids[clusterId];
            var nearestNodeId = _axiomReadRepository.GetNearestAxiom(clusterCentroid).Result.Id;

            var axiomsToUseForLabeling = axiomsInCluster
                .Select(a => a.AxiomText)
                .GetRandom(_percentOfAxiomsToUseForLabeling)
                .ToList();

            var centroidsOfOtherClusters = clusteringResult.Centroids
                .Where((c, index) => index != clusterId)
                .ToList();

            var axiomsOfOtherClusters = centroidsOfOtherClusters
                .Select(async c => await _axiomReadRepository.GetNearestAxiom(c))
                .Select(a => a.Result.AxiomText);

            var axiomList = string.Join("\r\n", axiomsToUseForLabeling.Select(a => $"* {a}"));
            var contrastingAxioms = string.Join("\r\n", axiomsOfOtherClusters.Select(a => $"* {a}"));
            var context = $"Target Cluster Axioms:\r\n{axiomList}\r\n\r\nReference Cluster Examples:\r\n{contrastingAxioms}\r\n\r\nTask:\r\nGenerate a single short label that captures the most distinctive unifying concept of the Target Cluster.\r\nUse the Reference Cluster Examples only to understand what themes belong to other clusters so that your label is not generic or overlapping.\r\n\r\nInstructions:\r\n- Keep the label concise.\r\n- Prefer one word.\r\n- If multiple words are needed, use camelCase.\r\n- Do not summarize the axioms.\r\n- Do not explain your reasoning.\r\n- Output only the label.";
            var chatResult = _chatClient.GetChatCompletion(context);

            var groupAxioms = axiomsInCluster.Select(a => new KeyValuePair<string, string>(a.Id, a.AxiomText));
            var group = new Cluster(clusterId, clusterCentroid, nearestNodeId, chatResult, groupAxioms);

            Console.WriteLine(group.ToString());
            results.Add(group);
        }

        return results;
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