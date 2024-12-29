namespace Regression.Interfaces;

public interface IVectorPrediction
{
    public int InputNodeCount { get; }

    public double[] Input { get; } // Size is based on input node count
    public double[] Predicted { get; }
    public double[]? Expected { get; }

    public double[]? Error { get; } // Predicted - Expected

    public double[]? WeightsErrors { get; } // Input[n] * Error
    public double[]? BiasErrors { get; }
}
