using Regression.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Regression.Extensions;

public static class LinearPredictionExtensions
{
    public static double[] CalculateWeightsErrorGradient(this IEnumerable<IScalarPrediction> predictions)
    {
        // Grab the nth element of each prediction's WeightsErrors array
        var depth = predictions.First().InputNodeCount;
        var gradients = new double[depth];

        // TODO: Handle the situation where the # of weights supplied doesn't match the # of input nodes

        // TODO: Handle the situation where the error is infinity
        for (int i = 0; i < depth; i++)
            gradients[i] = predictions
                .Where(p => p.WeightsErrors is not null)
                .Select(p => p.WeightsErrors![i])
                .ToArray()
                .CalculateGradient();

        return gradients;
    }

    public static double[] CalculateBiasErrorGradient(this IEnumerable<IScalarPrediction> predictions)
    {
        // Grab the nth element of each prediction's BiasErrors array
        var depth = predictions.First().InputNodeCount;
        var gradients = new double[depth];

        // TODO: Handle the situation where the error is infinity
        for (int i = 0; i < depth; i++)
            gradients[i] = predictions
                .Where(p => p.BiasErrors is not null)
                .Select(p => p.BiasErrors!.Value)
                .ToArray()
                .CalculateGradient();

        return gradients;
    }

    public static double CalculateMeanSquaredError(this IEnumerable<IScalarPrediction> predictions)
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
