namespace Regression.Interfaces;

public interface IPredictScalarValues
{
    public double[] Weights { get; }
    public double[] Biases { get; }


    double Predict(double[] x);

    double Test(IDictionary<double[], double> testSet);

    bool Train(IDictionary<double[], double> trainingSet, 
        double convergenceThreshold = Constants.Training.DefaultConvergenceThreshold, 
        Action<int, IPredictScalarValues, double>? callback = null);
}