namespace AxiomClusteringDemo.Clustering;

/// <summary>
/// Computes the mean Silhouette Score for a clustering result.
/// Range: [-1, 1]. Higher values indicate better-separated, more cohesive clusters.
/// </summary>
internal static class SilhouetteScore
{
    public static double Calculate(
        IReadOnlyList<IReadOnlyList<float>> vectors,
        IReadOnlyList<int> assignments)
    {
        int n = vectors.Count;
        int k = assignments.Max() + 1;

        if (k < 2)
        {
            return 0d;
        }

        List<int>[] clusters = new List<int>[k];
        for (int c = 0; c < k; c++)
        {
            clusters[c] = [];
        }

        for (int i = 0; i < n; i++)
        {
            clusters[assignments[i]].Add(i);
        }

        double total = 0d;
        for (int i = 0; i < n; i++)
        {
            int ci = assignments[i];

            if (clusters[ci].Count == 1)
            {
                // s(i) is defined as 0 for singleton clusters
                continue;
            }

            // a(i): mean intra-cluster distance to all other members
            double intraSum = 0d;
            foreach (int j in clusters[ci])
            {
                if (j != i)
                {
                    intraSum += VectorMath.EuclideanDistance(vectors[i], vectors[j]);
                }
            }

            double a = intraSum / (clusters[ci].Count - 1);

            // b(i): mean distance to the nearest other cluster
            double b = double.MaxValue;
            for (int c = 0; c < k; c++)
            {
                if (c == ci || clusters[c].Count == 0)
                {
                    continue;
                }

                double interSum = 0d;
                foreach (int j in clusters[c])
                {
                    interSum += VectorMath.EuclideanDistance(vectors[i], vectors[j]);
                }

                double meanInterDist = interSum / clusters[c].Count;
                if (meanInterDist < b)
                {
                    b = meanInterDist;
                }
            }

            double maxAB = Math.Max(a, b);
            if (maxAB > 0d)
            {
                total += (b - a) / maxAB;
            }
        }

        return total / n;
    }
}
