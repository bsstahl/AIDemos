﻿﻿﻿﻿﻿namespace GD.Extensions;

public static class VectorPredictionExtensions
{
    //public static double[] CalculateWeightsErrorGradient(this IEnumerable<VectorPrediction> predictions)
    //{
    //    var depth = predictions.First().InputNodeCount;
    //    return predictions.CalculateGradient(p => p.WeightsErrors, depth);
    //}

    internal static double[] CalculateWeightsErrorGradient(this VectorPrediction prediction)
    {
        if (prediction.Errors.Any(e => !e.HasValue))
            throw new ArgumentException("The expected and predicted values must be provided for all nodes.");

        int numPreviousLayerNeurons = prediction.Input.Length;
        int numCurrentLayerNeurons = prediction.Errors.Length;

        double[] weightGradients = new double[numPreviousLayerNeurons * numCurrentLayerNeurons];

        for (int j = 0; j < numCurrentLayerNeurons; j++)
        {
            // Negate delta since we're using (Expected - Predicted) for error
            double delta_j = -prediction.Errors[j]!.Value * prediction.Predicted[j].SigmoidDerivative();
            for (int i = 0; i < numPreviousLayerNeurons; i++)
            {
                var index = j * numPreviousLayerNeurons + i;
                weightGradients[index] = delta_j * prediction.Input[i];
            }
        }

        return weightGradients;
    }

    internal static double[] CalculateBiasesErrorGradient(this VectorPrediction prediction)
    {
        if (prediction.Errors.Any(e => !e.HasValue))
            throw new ArgumentException("The expected and predicted values must be provided for all nodes.");

        int numPreviousLayerNeurons = prediction.Input.Length;
        int numCurrentLayerNeurons = prediction.Errors.Length;

        double[] biasesGradients = new double[numCurrentLayerNeurons];

        for (int j = 0; j < numCurrentLayerNeurons; j++)
        {
            // Negate delta since we're using (Expected - Predicted) for error
            double delta_j = -prediction.Errors[j]!.Value * prediction.Predicted[j].SigmoidDerivative();
            biasesGradients[j] = delta_j;
        }

        return biasesGradients;
    }


    //public static double[] CalculateBiasErrorGradient(this IEnumerable<VectorPrediction> predictions)
    //{
    //    var depth = predictions.First().OutputNodeCount;
    //    return predictions.CalculateGradient(p => p.BiasesErrors, depth);
    //}

    //internal static double[] CalculateGradient(this IEnumerable<VectorPrediction> predictions, Func<VectorPrediction, double?[]> errors, int depth)
    //{
    //    // TODO: Handle the situation where the error is infinity
    //    var start = predictions.Select(p => errors.Invoke(p)).ToArray();
    //    var cleaned = start.Where(p => p.Clean().Length.Equals(depth)).Select(p => p.Clean()).ToArray();
    //    return cleaned.SelectMany(p => p).ToArray();
    //}

    public static double CalculateMeanSquaredError(this IEnumerable<VectorPrediction> predictions)
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

    //public static IDictionary<double[], double?[]> AsTrainingSet(this VectorPrediction[] predictions, double?[][] expectations)
    //{
    //    // Turns the predictions from one layer into a training set for the next layer

    //    if (!predictions.Length.Equals(expectations.Length))
    //        throw new ArgumentException("The number of predictions must match the number of expectations.");

    //    var results = new Dictionary<double[], double?[]>();
    //    for (int i = 0; i < predictions.Length; i++)
    //        results.Add(predictions[i].Predicted, expectations[i]);

    //    return results;
    //}
}
