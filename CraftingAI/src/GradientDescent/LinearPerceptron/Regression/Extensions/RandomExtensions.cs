namespace Regression.Extensions;

public static class RandomExtensions
{
    public static double GetRandomDouble(this Random r, double lowerBound, double upperBound)
    {
        // A random double between lowerBound and upperBound
        var delta = upperBound - lowerBound;
        return (delta * r.NextDouble()) + lowerBound;
    }
}
