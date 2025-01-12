namespace GD.Extensions;

public static class VectorExtensions
{
    public static double DotProduct(this double[] vectorA, double[] vectorB)
    {
        // Ensure both vectors have the same length
        if (vectorA.Length != vectorB.Length)
            throw new ArgumentException("Vectors must be of the same length.");

        // Calculate the dot product using LINQ
        return vectorA.Zip(vectorB, (a, b) => a * b).Sum();
    }
}
