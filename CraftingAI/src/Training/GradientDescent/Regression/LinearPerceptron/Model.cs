using Regression.Extensions;
using Regression.Interfaces;

namespace Regression.LinearPerceptron;

public class Model : IPredictScalarValues
{
    const int _maxIterations = 1000000;
    const double _learningRate = 0.0001;

    // -Max Double < Param < Max Double
    public double[] Weights { get; set; } 
    public double Bias { get; set; }

    public bool TrainingConverged { get; set; }
    public int TrainingIterations { get; set; }
    public double ConvergenceThreshold { get; set; }

    double[] IPredictScalarValues.Biases => [this.Bias];

    private readonly int _inputCount;
    private readonly Random _random = new();

    public Model(int inputCount)
    {
        _inputCount = inputCount;

        // Set parameters to start with small random values
        this.Weights = Enumerable.Range(0, inputCount).Select(_ => _random.GetRandomDouble(-0.05, 0.05)).ToArray();
        this.Bias = _random.GetRandomDouble(-0.05, 0.05);
    }

    public double Predict(double[] inputs)
    {
        // return FeatureWeightedParameters * x + RawParameters;
        throw new NotImplementedException();
    }

    private IEnumerable<ScalarPrediction> Predict(IDictionary<double[], double> trainingSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value
        var result = new List<ScalarPrediction>();
        foreach (var item in trainingSet)
        {
            var prediction = Predict(item.Key);
            result.Add(new ScalarPrediction(item.Key.Length, item.Key, prediction, item.Value));
        }
        return result;
    }

    public double Test(IDictionary<double[], double> testSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value
        // Then calculate the error for the test set based on those predictions
        var predictions = Predict(testSet);
        return predictions.CalculateMeanSquaredError();
    }

    public bool Train(IDictionary<double[], double> trainingSet, 
        double convergenceThreshold = Constants.Training.DefaultConvergenceThreshold, 
        Action<int, IPredictScalarValues, double>? callback = null)
    {
        // Verify that all training items have the correct number of inputs
        if (trainingSet.All(item => item.Key.Count().Equals(_inputCount)))
            throw new ArgumentException($"All training items must have {_inputCount} inputs", nameof(trainingSet));

        this.ConvergenceThreshold = convergenceThreshold;

        this.TrainingConverged = false;
        this.TrainingIterations = 0;
        while (!this.TrainingConverged && this.TrainingIterations < _maxIterations)
        {
            // Get predictions for the training set
            var predictions = this.Predict(trainingSet);

            // Compute Error Gradients
            var deltaM = predictions.CalculateWeightsErrorGradient();
            var deltaB = predictions.CalculateBiasErrorGradient();

            // Output intermediate results periodically
            if (callback is not null && (this.TrainingIterations < 100 || this.TrainingIterations % 100 == 0))
            {
                // Calculate the error for the test set
                var testError = predictions.CalculateMeanSquaredError();
                callback.Invoke(this.TrainingIterations, this.Clone(), testError);
            }

            // Update Model Parameters
            for (int i = 0; i < this.Weights.Length; i++)
                this.Weights[i] -= _learningRate * deltaM[i];
            this.Bias -= _learningRate * deltaB[0];

            // Determine if the model has converged
            var weightsConverged = true;
            for (int i = 0; i < this.Weights.Length; i++)
                weightsConverged &= Math.Abs(deltaM[i]) < this.ConvergenceThreshold;
            this.TrainingConverged = weightsConverged && Math.Abs(deltaB[0]) < this.ConvergenceThreshold;

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

    private Model Clone() => new Model(this.Weights.Length)
    {
        Weights = this.Weights,
        Bias = this.Bias,
        TrainingConverged = this.TrainingConverged,
        TrainingIterations = this.TrainingIterations,
        ConvergenceThreshold = this.ConvergenceThreshold
    };

}
