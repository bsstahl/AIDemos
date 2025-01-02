namespace GD.Test.Extensions;

public static class VectorTypeExtensions
{
    public static double[] GetVector(this VectorType type, int length)
    {
        return type switch
        {
            VectorType.AllZeros => new double[length],
            VectorType.AllPointFives => Enumerable.Repeat(0.5, length).ToArray(),
            VectorType.AllOnes => Enumerable.Repeat(1.0, length).ToArray(),
            _ => throw new NotImplementedException()
        };
    }
}
