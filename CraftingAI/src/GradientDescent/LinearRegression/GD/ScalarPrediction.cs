namespace GD;

public class ScalarPrediction
{
    public double Input { get; set; }
    public double Predicted { get; set; }
    public double? Expected { get; set; }

    public double? Error
        => Expected.HasValue ? Expected.Value - Predicted : null;

    public double? WeightError
        => Error.HasValue
            ? this.Input * this.Error.Value
            : null;

    public double? BiasError
        => Expected.HasValue ? Error : null;

    public ScalarPrediction(double input, double value, double? expected)
    {
        Input = input;
        Predicted = value;
        Expected = expected;
    }
}
