namespace AxiomClusteringDemo.Clustering;

/// <summary>
/// Computes the Davies-Bouldin Index for a clustering result.
/// Lower values indicate better-separated clusters with less intra-cluster scatter.
/// </summary>
internal static class DaviesBouldinIndex
{
    public static double Calculate(
        IReadOnlyList<IReadOnlyList<float>> vectors,
        IReadOnlyList<int> assignments,
        IReadOnlyList<IReadOnlyList<float>> centroids)
    {
        int k = centroids.Count;

        if (k < 2)
        {
            return 0d;
        }

        // scatter[c] = mean distance from each point in cluster c to its centroid
        double[] scatter = new double[k];
        int[] counts = new int[k];

        for (int i = 0; i < vectors.Count; i++)
        {
            int c = assignments[i];
            scatter[c] += VectorMath.EuclideanDistance(vectors[i], centroids[c]);
            counts[c]++;
        }

        for (int c = 0; c < k; c++)
        {
            if (counts[c] > 0)
            {
                scatter[c] /= counts[c];
            }
        }

        // DBI = mean over each cluster of max R(i,j) = (scatter[i] + scatter[j]) / dist(centroid_i, centroid_j)
        double total = 0d;
        for (int i = 0; i < k; i++)
        {
            double maxR = 0d;
            for (int j = 0; j < k; j++)
            {
                if (i == j)
                {
                    continue;
                }

                double centroidDist = VectorMath.EuclideanDistance(centroids[i], centroids[j]);
                if (centroidDist == 0d)
                {
                    continue;
                }

                double r = (scatter[i] + scatter[j]) / centroidDist;
                if (r > maxR)
                {
                    maxR = r;
                }
            }

            total += maxR;
        }

        return total / k;
    }
}
