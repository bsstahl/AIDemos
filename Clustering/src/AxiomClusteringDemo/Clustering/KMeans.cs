using System.Security.Cryptography;

namespace AxiomClusteringDemo.Clustering;

internal sealed class KMeans
{
    public int K { get; }

    public int MaxIterations { get; }

    public double ConvergenceTolerance { get; }

    public KMeans(int k, int maxIterations = 100, double convergenceTolerance = 1e-4)
    {
        if (k <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(k), "k must be greater than zero.");
        }

        if (maxIterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxIterations), "Max iterations must be greater than zero.");
        }

        if (convergenceTolerance < 0d)
        {
            throw new ArgumentOutOfRangeException(
                nameof(convergenceTolerance),
                "Convergence tolerance must be greater than or equal to zero.");
        }

        K = k;
        MaxIterations = maxIterations;
        ConvergenceTolerance = convergenceTolerance;
    }

    public KMeansResult Fit(IReadOnlyList<IReadOnlyList<float>> vectors)
    {
        ArgumentNullException.ThrowIfNull(vectors);

        if (vectors.Count == 0)
        {
            throw new ArgumentException("At least one vector is required for clustering.", nameof(vectors));
        }

        if (K > vectors.Count)
        {
            throw new ArgumentException("k cannot be larger than the number of vectors.", nameof(vectors));
        }

        int dimensions = vectors[0].Count;
        if (dimensions == 0)
        {
            throw new ArgumentException("Vectors must not be empty.", nameof(vectors));
        }

        ValidateDimensions(vectors, dimensions);

        float[][] centroids = InitializeCentroids(vectors);
        var assignments = new int[vectors.Count];
        Array.Fill(assignments, -1);

        bool converged = false;
        int iterations = 0;

        for (; iterations < MaxIterations; iterations++)
        {
            AssignClusters(vectors, centroids, assignments);
            float[][] updatedCentroids = RecomputeCentroids(vectors, assignments, dimensions);

            converged = HaveCentroidsConverged(centroids, updatedCentroids, ConvergenceTolerance);
            centroids = updatedCentroids;

            if (converged)
            {
                iterations++;
                break;
            }
        }

        if (!converged && iterations == MaxIterations)
        {
            iterations = MaxIterations;
        }

        IReadOnlyList<IReadOnlyList<float>> centroidResults = centroids
            .Select(static centroid => (IReadOnlyList<float>)centroid)
            .ToArray();

        return new KMeansResult(assignments, centroidResults, iterations, converged);
    }

    private static void ValidateDimensions(IReadOnlyList<IReadOnlyList<float>> vectors, int dimensions)
    {
        for (int i = 1; i < vectors.Count; i++)
        {
            if (vectors[i].Count != dimensions)
            {
                throw new ArgumentException(
                    $"Vector at index {i} has dimension {vectors[i].Count}, expected {dimensions}.",
                    nameof(vectors));
            }
        }
    }

    private float[][] InitializeCentroids(IReadOnlyList<IReadOnlyList<float>> vectors)
    {
        var centroids = new float[K][];
        var selectedIndices = new HashSet<int>();

        for (int i = 0; i < K; i++)
        {
            int index;
            do
            {
                index = RandomNumberGenerator.GetInt32(vectors.Count);
            }
            while (!selectedIndices.Add(index));

            centroids[i] = CopyVector(vectors[index]);
        }

        return centroids;
    }

    private static void AssignClusters(IReadOnlyList<IReadOnlyList<float>> vectors, float[][] centroids, int[] assignments)
    {
        for (int vectorIndex = 0; vectorIndex < vectors.Count; vectorIndex++)
        {
            int bestClusterIndex = 0;
            double bestDistance = VectorMath.EuclideanDistance(vectors[vectorIndex], centroids[0]);

            for (int clusterIndex = 1; clusterIndex < centroids.Length; clusterIndex++)
            {
                double distance = VectorMath.EuclideanDistance(vectors[vectorIndex], centroids[clusterIndex]);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestClusterIndex = clusterIndex;
                }
            }

            assignments[vectorIndex] = bestClusterIndex;
        }
    }

    private float[][] RecomputeCentroids(IReadOnlyList<IReadOnlyList<float>> vectors, int[] assignments, int dimensions)
    {
        var sums = new float[K][];
        var counts = new int[K];

        for (int cluster = 0; cluster < K; cluster++)
        {
            sums[cluster] = new float[dimensions];
        }

        for (int vectorIndex = 0; vectorIndex < vectors.Count; vectorIndex++)
        {
            int cluster = assignments[vectorIndex];
            counts[cluster]++;

            for (int dimension = 0; dimension < dimensions; dimension++)
            {
                sums[cluster][dimension] += vectors[vectorIndex][dimension];
            }
        }

        for (int cluster = 0; cluster < K; cluster++)
        {
            if (counts[cluster] == 0)
            {
                sums[cluster] = CopyVector(vectors[RandomNumberGenerator.GetInt32(vectors.Count)]);
                continue;
            }

            float divisor = counts[cluster];
            for (int dimension = 0; dimension < dimensions; dimension++)
            {
                sums[cluster][dimension] /= divisor;
            }
        }

        return sums;
    }

    private static bool HaveCentroidsConverged(float[][] previousCentroids, float[][] currentCentroids, double tolerance)
    {
        for (int i = 0; i < previousCentroids.Length; i++)
        {
            if (VectorMath.EuclideanDistance(previousCentroids[i], currentCentroids[i]) > tolerance)
            {
                return false;
            }
        }

        return true;
    }

    private static float[] CopyVector(IReadOnlyList<float> source)
    {
        var copy = new float[source.Count];
        for (int i = 0; i < source.Count; i++)
        {
            copy[i] = source[i];
        }

        return copy;
    }
}
