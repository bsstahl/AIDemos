using Regression.Extensions;

namespace Regression;

public class LinearModel
{
    const int _maxIterations = 1000000;
    const double _learningRate = 0.0001;
    const double _defaultConvergenceThreshold = 0.0000000001;

    public double M { get; set; } // -Max Double < M < Max Double
    public double B { get; set; } // -Max Double < B < Max Double

    public bool TrainingConverged { get; set; } = false;
    public int TrainingIterations { get; set; } = 0;
    public double ConvergenceThreshold { get; set; } = _defaultConvergenceThreshold;

    public double Predict(double x) => this.M * x + this.B;


    private IEnumerable<LinearPrediction> Predict(IDictionary<double, double> trainingSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value
        var result = new List<LinearPrediction>();
        foreach (var item in trainingSet)
        {
            var prediction = this.Predict(item.Key);
            result.Add(new LinearPrediction(item.Key, prediction, item.Value));
        }
        return result;
    }

    public double Test(IDictionary<double, double> testSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value
        // Then calculate the error for the test set based on those predictions
        var predictions = this.Predict(testSet);
        return predictions.CalculateMeanSquaredError();
    }

    public static LinearModel Train(IDictionary<double, double> trainingSet, double convergenceThreshold = _defaultConvergenceThreshold, Action<int, LinearModel, double>? callback = null)
    {
        Random random = new();

        // Note: With linear regression, there are simpler ways to do this
        // but we are using it as an example of training a model
        // so we are using a process that generalizes to other models

        // Set parameters to start with small random values
        var model = new LinearModel()
        {
            M = random.GetRandomDouble(-0.05, 0.05),
            B = random.GetRandomDouble(-0.05, 0.05),
            ConvergenceThreshold = convergenceThreshold
        };

        model.TrainingConverged = false;
        model.TrainingIterations = 0;
        while (!model.TrainingConverged && model.TrainingIterations < _maxIterations)
        {
            // Get predictions for the training set
            var predictions = model.Predict(trainingSet);

            // Compute Error Gradients
            var deltaM = predictions.CalculateFeatureWeightedErrorGradient();
            var deltaB = predictions.CalculateRawErrorGradient();

            // Output intermediate results periodically
            if (callback is not null && (model.TrainingIterations < 100 || model.TrainingIterations % 100 == 0))
            {
                // Calculate the error for the test set
                var testError = predictions.CalculateMeanSquaredError();
                callback.Invoke(model.TrainingIterations, model, testError);
            }

            // Update Model Parameters
            model.M -= _learningRate * deltaM;
            model.B -= _learningRate * deltaB;

            // Determine if the model has converged
            model.TrainingConverged = Math.Abs(deltaM) < model.ConvergenceThreshold && Math.Abs(deltaB) < model.ConvergenceThreshold;

            model.TrainingIterations++;

            if (model.TrainingConverged && callback is not null)
            {
                // Calculate the error for the test set
                var testError = predictions.CalculateMeanSquaredError();
                callback.Invoke(model.TrainingIterations, model, testError);
            }
        }

        return model;
    }

}
