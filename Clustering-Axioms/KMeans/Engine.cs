namespace KMeans;

public sealed class Engine
{
    private readonly Random _random;

    public Engine(int? randomSeed = null)
    {
        _random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
    }

    public Task<KMeansResult> Fit(IReadOnlyList<VectorPoint> points, int k, int maxIterations = 100, float tolerance = 1e-4f)
    {
        if (k <= 0) throw new ArgumentOutOfRangeException(nameof(k));
        if (maxIterations <= 0) throw new ArgumentOutOfRangeException(nameof(maxIterations));
        if (tolerance < 0) throw new ArgumentOutOfRangeException(nameof(tolerance));

        if (points == null) throw new ArgumentNullException(nameof(points));
        if (points.Count < k) throw new ArgumentException("Number of points must be >= k.", nameof(points));

        int dim = points[0].Features.Length;
        EnsureAllSameDimension(points, dim);

        // 1. Initialize centroids (simple: pick k random points)
        var centroids = this.InitializeCentroids(points, dim, k);

        // cluster assignment: point index -> cluster index
        var assignments = new int[points.Count];

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            bool changed = AssignPointsToClusters(points, centroids, assignments);
            var newCentroids = RecomputeCentroids(points, assignments, centroids, dim);
            float maxShift = MaxCentroidShift(centroids, newCentroids);

            centroids = newCentroids;
            if (!changed || maxShift < tolerance)
            {
                // Converged
                break;
            }
        }

        // Build readable result: map from Id -> cluster index
        var assignmentsById = new Dictionary<string, int>(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            assignmentsById[points[i].Id] = assignments[i];
        }

        return Task.FromResult(new KMeansResult(centroids, assignmentsById));
    }

    public Task<float> GetSilhouetteScore(IReadOnlyList<VectorPoint> points, KMeansResult result)
    {
        // Extract vectors and labels in aligned order
        var vectors = points.Select(p => p.Features).ToList();
        var labels = points.Select(p => result.AssignmentsById[p.Id]).ToList();

        int n = vectors.Count;
        float total = 0f;

        for (int i = 0; i < n; i++)
        {
            int cluster = labels[i];

            float a = AverageIntraClusterDistance(i, vectors, labels, cluster);
            float b = AverageNearestOtherClusterDistance(i, vectors, labels, cluster);

            float s = (b - a) / Math.Max(a, b);
            total += s;
        }

        return Task.FromResult(total / n);
    }

    private static void EnsureAllSameDimension(IReadOnlyList<VectorPoint> points, int dim)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].Features.Length != dim)
            {
                throw new InvalidOperationException(
                    $"Point '{points[i].Id}' has dimension {points[i].Features.Length}, expected {dim}.");
            }
        }
    }

    private List<float[]> InitializeCentroids(IReadOnlyList<VectorPoint> points, int dim, int k)
    {
        // Simple and readable: randomly pick k distinct points as initial centroids
        var indices = Enumerable.Range(0, points.Count).ToList();
        Shuffle(indices);

        var centroids = new List<float[]>(k);
        for (int i = 0; i < k; i++)
        {
            centroids.Add((float[])points[indices[i]].Features.Clone());
        }

        return centroids;
    }

    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static bool AssignPointsToClusters(
        IReadOnlyList<VectorPoint> points,
        IReadOnlyList<float[]> centroids,
        int[] assignments)
    {
        bool anyChanged = false;

        for (int i = 0; i < points.Count; i++)
        {
            int bestCluster = -1;
            float bestDistance = float.MaxValue;

            for (int c = 0; c < centroids.Count; c++)
            {
                float distance = SquaredEuclidean(points[i].Features, centroids[c]);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCluster = c;
                }
            }

            if (assignments[i] != bestCluster)
            {
                assignments[i] = bestCluster;
                anyChanged = true;
            }
        }

        return anyChanged;
    }

    private static List<float[]> RecomputeCentroids(
        IReadOnlyList<VectorPoint> points,
        int[] assignments,
        IReadOnlyList<float[]> oldCentroids,
        int dim)
    {
        int k = oldCentroids.Count;

        var newCentroids = new List<float[]>(k);
        var counts = new int[k];

        // Initialize accumulators
        for (int c = 0; c < k; c++)
        {
            newCentroids.Add(new float[dim]);
        }

        // Sum points per cluster
        for (int i = 0; i < points.Count; i++)
        {
            int cluster = assignments[i];
            counts[cluster]++;

            var features = points[i].Features;
            var centroid = newCentroids[cluster];

            for (int d = 0; d < dim; d++)
            {
                centroid[d] += features[d];
            }
        }

        // Divide by counts to get the mean
        for (int c = 0; c < k; c++)
        {
            if (counts[c] == 0)
            {
                // Empty cluster: keep the old centroid
                newCentroids[c] = (float[])oldCentroids[c].Clone();
            }
            else
            {
                var centroid = newCentroids[c];
                float count = counts[c];
                for (int d = 0; d < dim; d++)
                {
                    centroid[d] /= count;
                }
            }
        }

        return newCentroids;
    }

    private static float MaxCentroidShift(
        IReadOnlyList<float[]> oldCentroids,
        IReadOnlyList<float[]> newCentroids)
    {
        float maxShift = 0f;

        for (int c = 0; c < oldCentroids.Count; c++)
        {
            float shift = (float)Math.Sqrt(SquaredEuclidean(oldCentroids[c], newCentroids[c]));
            if (shift > maxShift)
            {
                maxShift = shift;
            }
        }

        return maxShift;
    }

    private static float SquaredEuclidean(float[] a, float[] b)
    {
        float sum = 0f;
        for (int i = 0; i < a.Length; i++)
        {
            float diff = a[i] - b[i];
            sum += diff * diff;
        }
        return sum;
    }

    private static float AverageIntraClusterDistance(
        int index,
        IReadOnlyList<float[]> vectors,
        IReadOnlyList<int> labels,
        int cluster)
    {
        float sum = 0f;
        int count = 0;

        for (int j = 0; j < vectors.Count; j++)
        {
            if (j == index) continue;
            if (labels[j] != cluster) continue;

            sum += SquaredEuclidean(vectors[index], vectors[j]);
            count++;
        }

        return count == 0 ? 0f : sum / count;
    }

    private static float AverageNearestOtherClusterDistance(
        int index,
        IReadOnlyList<float[]> vectors,
        IReadOnlyList<int> labels,
        int ownCluster)
    {
        var clusterDistances = new Dictionary<int, (float sum, int count)>();

        for (int j = 0; j < vectors.Count; j++)
        {
            int c = labels[j];
            if (c == ownCluster) continue;

            float d = SquaredEuclidean(vectors[index], vectors[j]);

            if (!clusterDistances.ContainsKey(c))
                clusterDistances[c] = (0f, 0);

            var (sum, count) = clusterDistances[c];
            clusterDistances[c] = (sum + d, count + 1);
        }

        float best = float.MaxValue;

        foreach (var (_, value) in clusterDistances)
        {
            var (sum, count) = value;
            float avg = sum / count;
            if (avg < best)
                best = avg;
        }

        return best;
    }

}