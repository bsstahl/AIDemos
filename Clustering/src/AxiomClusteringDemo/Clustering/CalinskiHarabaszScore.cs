namespace AxiomClusteringDemo.Clustering;

/// <summary>
/// Computes the Calinski-Harabasz Score (Variance Ratio Criterion) for a clustering result.
/// Higher values indicate more compact, well-separated clusters.
/// </summary>
internal static class CalinskiHarabaszScore
{
    public static double Calculate(
        IReadOnlyList<IReadOnlyList<float>> vectors,
        IReadOnlyList<int> assignments,
        IReadOnlyList<IReadOnlyList<float>> centroids)
    {
        int n = vectors.Count;
        int k = centroids.Count;

        if (k < 2 || n <= k)
        {
            return 0d;
        }

        int dimensions = vectors[0].Count;

        // Global centroid
        double[] globalCentroid = new double[dimensions];
        for (int i = 0; i < n; i++)
        {
            for (int d = 0; d < dimensions; d++)
            {
                globalCentroid[d] += vectors[i][d];
            }
        }

        for (int d = 0; d < dimensions; d++)
        {
            globalCentroid[d] /= n;
        }

        // Cluster sizes
        int[] counts = new int[k];
        for (int i = 0; i < n; i++)
        {
            counts[assignments[i]]++;
        }

        // BGSS: between-group sum of squares — weighted squared distance of each centroid to the global centroid
        double bgss = 0d;
        for (int c = 0; c < k; c++)
        {
            double squaredDist = 0d;
            for (int d = 0; d < dimensions; d++)
            {
                double delta = centroids[c][d] - globalCentroid[d];
                squaredDist += delta * delta;
            }

            bgss += counts[c] * squaredDist;
        }

        // WGSS: within-group sum of squares — sum of squared distances from each point to its centroid
        double wgss = 0d;
        for (int i = 0; i < n; i++)
        {
            int c = assignments[i];
            for (int d = 0; d < dimensions; d++)
            {
                double delta = vectors[i][d] - centroids[c][d];
                wgss += delta * delta;
            }
        }

        if (wgss == 0d)
        {
            return double.PositiveInfinity;
        }

        return (bgss / (k - 1)) / (wgss / (n - k));
    }
}
