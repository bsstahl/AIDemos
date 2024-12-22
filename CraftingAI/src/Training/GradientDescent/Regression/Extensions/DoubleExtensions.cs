namespace Regression.Extensions;

public static class DoubleExtensions
{
    public static double CalculateGradient(this double[] errors)
    {
        var sumOfErrors = errors.Sum();
        var scalingFactor = -2.0 / errors.Count();
        return scalingFactor * sumOfErrors;
    }
}
