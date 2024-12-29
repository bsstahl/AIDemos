using Regression.Extensions;
using Regression.Interfaces;
using System.Diagnostics;

namespace Regression.Linear;

public class Model : IPredictScalarValues
{
    const int _maxIterations = 1000000;
    const double _learningRate = 0.0001;

    public double M { get; set; } // -Max Double < M < Max Double
    public double B { get; set; } // -Max Double < B < Max Double

    public bool TrainingConverged { get; set; }
    public int TrainingIterations { get; set; }
    public double ConvergenceThreshold { get; set; }
    public IActivateNeurons ActivationFunction { get; set; }

    double[] IPredictScalarValues.Weights => [this.M];
    double[] IPredictScalarValues.Biases => [this.B];

    public double Predict(double x) => this.ActivationFunction.Activate(M * x + B);
    public double Predict(double[] x) => Predict(x[0]);


    public Model(IActivateNeurons? activationFunction = null)
    {
        this.ActivationFunction = activationFunction ?? new Activations.None();
    }

    private IEnumerable<ScalarPrediction> Predict(IDictionary<double[], double> trainingSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value

        // Since this is a Linear Regression, we know there is only 1 input value
        trainingSet.ToList().ForEach(item => Debug.Assert(item.Key.Length == 1));

        var result = new List<ScalarPrediction>();
        foreach (var item in trainingSet)
        {
            var prediction = Predict(item.Key);
            result.Add(new ScalarPrediction(item.Key.Length, item.Key, prediction, item.Value));
        }
        return result;
    }

    public (double, IEnumerable<IScalarPrediction>) Test(IDictionary<double[], double> testSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value
        // Then calculate the error for the test set based on those predictions
        var predictions = Predict(testSet);
        return (predictions.CalculateMeanSquaredError(), predictions);
    }

    public bool Train(IDictionary<double[], double> trainingSet, 
        double convergenceThreshold = Constants.Training.DefaultConvergenceThreshold, 
        Action<int, IPredictScalarValues, double>? callback = null)
    {
        Random random = new();

        // Note: With linear regression, there are simpler ways to do this
        // but we are using it as an example of training a model
        // so we are using a process that generalizes to other models

        // Set parameters to start with small random values
        this.M = random.GetRandomDouble(-0.05, 0.05);
        this.B = random.GetRandomDouble(-0.05, 0.05);
        this.ConvergenceThreshold = convergenceThreshold;

        this.TrainingConverged = false;
        this.TrainingIterations = 0;
        while (!this.TrainingConverged && this.TrainingIterations < _maxIterations)
        {
            // Get predictions for the training set
            var predictions = this.Predict(trainingSet);

            // Compute Error Gradients
            var deltaM = predictions.CalculateWeightsErrorGradient()[0];
            var deltaB = predictions.CalculateBiasErrorGradient()[0];

            // Output intermediate results periodically
            if (callback is not null && (this.TrainingIterations < 100 || this.TrainingIterations % 1000 == 0))
            {
                // Calculate the error for the test set
                var testError = predictions.CalculateMeanSquaredError();
                callback.Invoke(this.TrainingIterations, this.Clone(), testError);
            }

            // Update Model Parameters
            this.M -= _learningRate * deltaM;
            this.B -= _learningRate * deltaB;

            // Determine if the model has converged
            this.TrainingConverged = Math.Abs(deltaM) < this.ConvergenceThreshold && Math.Abs(deltaB) < this.ConvergenceThreshold;

            this.TrainingIterations++;

            if (this.TrainingConverged && callback is not null)
            {
                // Calculate the error for the test set
                var testError = predictions.CalculateMeanSquaredError();
                callback.Invoke(this.TrainingIterations, this.Clone(), testError);
            }
        }

        return this.TrainingConverged;
    }

    private Model Clone() => new Model() 
    { 
        M = this.M, 
        B = this.B, 
        TrainingConverged = this.TrainingConverged, 
        TrainingIterations = this.TrainingIterations, 
        ConvergenceThreshold = this.ConvergenceThreshold 
    };

}
