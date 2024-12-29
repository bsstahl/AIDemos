namespace GD;

public class ScalarPrediction
{
    public int InputNodeCount { get; }

    public double[] Input { get; set; }
    public double Predicted { get; set; }
    public double? Expected { get; set; }

    public double? Error
        => Expected.HasValue ? Expected.Value - Predicted : null;

    public double[]? WeightsErrors
        => Error.HasValue
            ? Input.Select(i => i * Error.Value).ToArray()
            : null;

    public double? BiasErrors
        => Expected.HasValue ? Error : null;

    public ScalarPrediction(int inputNodeCount, double[] input, double value, double? expected)
    {
        InputNodeCount = inputNodeCount;
        Input = input;
        Predicted = value;
        Expected = expected;
    }
}
