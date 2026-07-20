namespace AxiomClusteringDemo.Clustering;

internal static class VectorMath
{
    public static double EuclideanDistance(IReadOnlyList<float> left, IReadOnlyList<float> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        if (left.Count != right.Count)
        {
            throw new ArgumentException("Vectors must have the same dimensionality.", nameof(right));
        }

        double sum = 0d;
        for (int i = 0; i < left.Count; i++)
        {
            double delta = left[i] - right[i];
            sum += delta * delta;
        }

        return Math.Sqrt(sum);
    }
}
