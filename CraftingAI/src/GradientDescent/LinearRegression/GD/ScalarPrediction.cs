namespace GD;

public class ScalarPrediction
{
    public double Input { get; set; }
    public double Predicted { get; set; }
    public double? Expected { get; set; }

    public double? Error
        => this.Expected.HasValue ? this.Expected.Value - this.Predicted : null;

    public double? WeightError
        => this.Error.HasValue
            ? this.Input * this.Error.Value
            : null;

    public double? BiasError
        => this.Expected.HasValue ? this.Error : null;

    public ScalarPrediction(double input, double value, double? expected)
    {
        Input = input;
        Predicted = value;
        Expected = expected;
    }
}
