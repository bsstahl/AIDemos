namespace GD;

public class VectorPrediction
{
    public int InputNodeCount { get; }
    public int OutputNodeCount { get; }

    public double[] Input { get; set; }
    public double[] Predicted { get; set; }
    public double[]? Expected { get; set; }

    //public double? Error
    //    => Expected.HasValue ? Expected.Value - Predicted : null;

    //public double[]? WeightsErrors
    //    => Error.HasValue
    //        ? Input.Select(i => i * Error.Value).ToArray()
    //        : null;

    //public double? BiasesErrors
    //    => Expected.HasValue ? Error : null;

    public VectorPrediction(int inputNodeCount, int outputNodeCount, double[] input, double[] value, double[]? expected)
    {
        InputNodeCount = inputNodeCount;
        OutputNodeCount = outputNodeCount;

        Input = input;
        Predicted = value;
        Expected = expected;
    }
}
