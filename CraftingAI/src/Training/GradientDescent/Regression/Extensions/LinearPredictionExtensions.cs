namespace Regression.Extensions;

public static class LinearPredictionExtensions
{
    public static double CalculateFeatureWeightedErrorGradient(this IEnumerable<LinearPrediction> predictions)
    {
        var errors = predictions
            .Where(p => p.FeatureWeightedError is not null && p.FeatureWeightedError is not double.NegativeInfinity && p.FeatureWeightedError is not double.NegativeInfinity)
            .Select(p => p.FeatureWeightedError!.Value);
        return errors.CalculateGradient();
    }

    public static double CalculateRawErrorGradient(this IEnumerable<LinearPrediction> predictions)
    {
        var errors = predictions
            .Where(p => p.RawError is not null && p.RawError is not double.NegativeInfinity && p.RawError is not double.PositiveInfinity)
            .Select(p => p.RawError!.Value);
        return errors.CalculateGradient();
    }

    public static double CalculateMeanSquaredError(this IEnumerable<LinearPrediction> predictions)
    {
        var errors = predictions
            .Where(p => p.RawError is not null & p.RawError is not double.NegativeInfinity && p.RawError is not double.PositiveInfinity)
            .Select(p => p.RawError!.Value);
        var squaredErrors = errors.Sum(e => Math.Pow(e, 2));
        var scalingFactor = 1.0 / errors.Count();
        return scalingFactor * squaredErrors;
    }
}
