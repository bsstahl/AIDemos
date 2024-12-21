namespace Regression;

public class LinearPrediction
{
    public double Input { get; set; }
    public double Predicted { get; set; }
    public double? Actual { get; set; }

    public double? FeatureWeightedError 
        => this.Actual.HasValue ? this.Input * (this.Actual.Value - this.Predicted) : null;

    public double? RawError
        => this.Actual.HasValue ? (this.Actual.Value - this.Predicted) : null;


    //public LinearPrediction(double input, double value)
    //    : this(input, value, null)
    //{ }

    public LinearPrediction(double input, double value, double? expected)
    {
        this.Input = input;
        this.Predicted = value;
        this.Actual = expected;
    }
}
