namespace Regression.Interfaces;

public interface IScalarPrediction
{
    public int InputNodeCount { get; }

    public double[] Input { get; } // Size is based on input node count
    public double Predicted { get; } // Scalar since we are only making 1 prediction
    public double? Expected { get; } // Scalar since we are only making 1 prediction

    public double? Error { get; } // Predicted - Expected

    public double[]? WeightsErrors { get; } // Input[n] * Error
    public double? BiasErrors { get; } // Just the scalar error since we are only making 1 prediction
}
