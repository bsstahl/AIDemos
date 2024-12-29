namespace Regression.Interfaces;

public interface IPredictScalarValues
{
    public double[] Weights { get; }
    public double[] Biases { get; }

    public IActivateNeurons ActivationFunction { get; set; }

    double Predict(double[] x);

    (double, IEnumerable<IScalarPrediction>) Test(IDictionary<double[], double> testSet);

    bool Train(IDictionary<double[], double> trainingSet, 
        double convergenceThreshold = Constants.Training.DefaultConvergenceThreshold, 
        Action<int, IPredictScalarValues, double>? callback = null);
}