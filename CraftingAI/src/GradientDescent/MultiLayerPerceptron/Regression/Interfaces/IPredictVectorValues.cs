namespace Regression.Interfaces;

public interface IPredictVectorValues
{
    public double[] Weights { get; }
    public double[] Biases { get; }

    public IActivateNeurons ActivationFunction { get; set; }

    double[] Predict(double[] x);

    (double[], IEnumerable<IVectorPrediction>) Test(IDictionary<double[], double> testSet);

    bool Train(IDictionary<double[], double> trainingSet, 
        double convergenceThreshold = Constants.Training.DefaultConvergenceThreshold, 
        Action<int, IPredictVectorValues, double>? callback = null);
}