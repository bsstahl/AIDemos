namespace GD.Extensions;

public static class LinearPredictionExtensions
{
    public static double CalculateWeightsErrorGradient(this IEnumerable<ScalarPrediction> predictions)
    {
        // TODO: Handle the situation where the error is infinity
        return predictions
            .Where(p => p.WeightError is not null)
            .Select(p => p.WeightError!.Value)
            .ToArray()
            .CalculateGradient();
    }

    public static double CalculateBiasErrorGradient(this IEnumerable<ScalarPrediction> predictions)
    {
        // TODO: Handle the situation where the error is infinity
        return predictions
            .Where(p => p.BiasError is not null)
            .Select(p => p.BiasError!.Value)
            .ToArray()
            .CalculateGradient();
    }

    public static double CalculateMeanSquaredError(this IEnumerable<ScalarPrediction> predictions)
    {
        var errors = predictions
            .Where(p => p.Error is not null && p.Error.HasValue)
            .Select(p => p.Error!.Value);

        // TODO: Handle the situation where not all predictions have a calculable error
        // (i.e. Expected is null or error is infinity)

        var squaredErrors = errors.Sum(e => Math.Pow(e, 2));
        var scalingFactor = 1.0 / errors.Count();
        return scalingFactor * squaredErrors;
    }
}
