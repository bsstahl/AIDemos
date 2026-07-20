using System.Globalization;
using System.Text.Json;
using AxiomClusteringDemo.Clustering;
using AxiomClusteringDemo.Models;

const int defaultClusterCount = 4;
const string defaultRelativeDataPath = "Clustering\\Data\\axioms.json";
const string noAxiomsFoundMessage = "No axioms were found in the input file.";

int k = ParseClusterCount(args, defaultClusterCount);
string? providedPath = args.Length > 1 ? args[1] : null;
string dataPath = ResolveDataPath(providedPath, defaultRelativeDataPath);

var axioms = await LoadAxiomsAsync(dataPath).ConfigureAwait(false);
if (axioms.Count == 0)
{
    Console.WriteLine(noAxiomsFoundMessage);
    return;
}

IReadOnlyList<IReadOnlyList<float>> vectors = axioms.Select(static axiom => axiom.Embedding).ToArray();
var kMeans = new KMeans(k);
KMeansResult result = kMeans.Fit(vectors);

double silhouette = SilhouetteScore.Calculate(vectors, result.Assignments);
double daviesBouldin = DaviesBouldinIndex.Calculate(vectors, result.Assignments, result.Centroids);
double calinskiHarabasz = CalinskiHarabaszScore.Calculate(vectors, result.Assignments, result.Centroids);

PrintSummary(axioms, result, silhouette, daviesBouldin, calinskiHarabasz);

static int ParseClusterCount(IReadOnlyList<string> args, int defaultClusterCount)
{
    if (args.Count == 0)
    {
        return defaultClusterCount;
    }

    if (int.TryParse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedK) && parsedK > 0)
    {
        return parsedK;
    }

    Console.WriteLine($"Invalid cluster count '{args[0]}'. Falling back to default k={defaultClusterCount}.");
    return defaultClusterCount;
}

static string ResolveDataPath(string? providedPath, string defaultRelativeDataPath)
{
    if (!string.IsNullOrWhiteSpace(providedPath))
    {
        string fullProvidedPath = Path.GetFullPath(providedPath);
        if (File.Exists(fullProvidedPath))
        {
            return fullProvidedPath;
        }

        throw new FileNotFoundException($"The provided input file was not found: {fullProvidedPath}", fullProvidedPath);
    }

    string cwdCandidate = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, defaultRelativeDataPath));
    if (File.Exists(cwdCandidate))
    {
        return cwdCandidate;
    }

    var startDirectory = new DirectoryInfo(AppContext.BaseDirectory);
    for (DirectoryInfo? dir = startDirectory; dir is not null; dir = dir.Parent)
    {
        string candidate = Path.Combine(dir.FullName, defaultRelativeDataPath);
        if (File.Exists(candidate))
        {
            return candidate;
        }
    }

    throw new FileNotFoundException(
        $"Could not find axioms data file. Expected at '{defaultRelativeDataPath}' relative to repository root, or pass an explicit path as the second argument.");
}

static async Task<IReadOnlyList<Axiom>> LoadAxiomsAsync(string dataPath)
{
    using FileStream stream = File.OpenRead(dataPath);
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    Axiom[]? axioms = await JsonSerializer.DeserializeAsync<Axiom[]>(stream, options).ConfigureAwait(false);
    if (axioms is null)
    {
        throw new InvalidDataException($"Input JSON did not contain a valid axiom array: {dataPath}");
    }

    EnsureValidEmbeddings(axioms);
    return axioms;
}

static void EnsureValidEmbeddings(IReadOnlyList<Axiom> axioms)
{
    const int expectedDimension = 768;
    for (int i = 0; i < axioms.Count; i++)
    {
        Axiom axiom = axioms[i];
        if (axiom.Embedding.Count != expectedDimension)
        {
            throw new InvalidDataException(
                $"Axiom at index {i} has embedding length {axiom.Embedding.Count}; expected {expectedDimension}.");
        }
    }
}

static void PrintSummary(
    IReadOnlyList<Axiom> axioms,
    KMeansResult result,
    double silhouette,
    double daviesBouldin,
    double calinskiHarabasz)
{
    Console.WriteLine($"k-means completed in {result.Iterations} iteration(s). Converged: {result.Converged}");
    Console.WriteLine();
    Console.WriteLine("Cluster Quality Scores");
    Console.WriteLine(new string('-', 72));
    Console.WriteLine($"  Silhouette Score:        {silhouette,8:F4}  (higher is better, range: -1..1)");
    Console.WriteLine($"  Davies-Bouldin Index:    {daviesBouldin,8:F4}  (lower is better)");

    string chDisplay = double.IsPositiveInfinity(calinskiHarabasz)
        ? "     ∞  (perfect cluster separation)"
        : $"{calinskiHarabasz,8:F2}  (higher is better)";

    Console.WriteLine($"  Calinski-Harabasz Score: {chDisplay}");
    Console.WriteLine();

    for (int clusterId = 0; clusterId < result.Centroids.Count; clusterId++)
    {
        Console.WriteLine($"Cluster {clusterId}");
        Console.WriteLine(new string('-', 72));

        int[] memberIndices = result.Assignments
            .Select(static (assignment, index) => (assignment, index))
            .Where(pair => pair.assignment == clusterId)
            .Select(static pair => pair.index)
            .ToArray();

        if (memberIndices.Length == 0)
        {
            Console.WriteLine("  (no axioms assigned)");
        }
        else
        {
            // foreach (int index in memberIndices)
            // {
            //     Console.WriteLine($"  - {axioms[index].Text}");
            // }

            int representativeIndex = FindClosestAxiomIndex(memberIndices, axioms, result.Centroids[clusterId]);
            Console.WriteLine();
            Console.WriteLine($"  Representative: {axioms[representativeIndex].Text}");
        }

        Console.WriteLine();
    }
}

static int FindClosestAxiomIndex(int[] memberIndices, IReadOnlyList<Axiom> axioms, IReadOnlyList<float> centroid)
{
    int closest = memberIndices[0];
    double bestDistance = VectorMath.EuclideanDistance(axioms[memberIndices[0]].Embedding, centroid);

    for (int i = 1; i < memberIndices.Length; i++)
    {
        double distance = VectorMath.EuclideanDistance(axioms[memberIndices[i]].Embedding, centroid);
        if (distance < bestDistance)
        {
            bestDistance = distance;
            closest = memberIndices[i];
        }
    }

    return closest;
}
