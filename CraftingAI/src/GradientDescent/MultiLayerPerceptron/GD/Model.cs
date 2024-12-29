using GD.Extensions;
using GD.Interfaces;

namespace GD;

public class Model
{
    const int _maxIterations = 1000000;
    const double _learningRate = 0.001;

    // TODO: Pull the weights and biases from the layers
    public double[] Weights => throw new NotImplementedException();
    public double[] Biases => throw new NotImplementedException();

    public bool TrainingConverged { get; set; }
    public int TrainingIterations { get; set; }
    public double ConvergenceThreshold { get; set; }

    public IActivateNeurons ActivationFunction { get; set; }

    private readonly int _inputCount;

    // Model Layers
    private readonly InputLayer _inputLayer;
    private readonly OutputLayer _outputLayer;

    public Model(int inputCount, int hiddenLayerNodes, double[] startingWeights, double[] startingBiases, IActivateNeurons? activationFunction = null)
    {
        _inputCount = inputCount;
        this.ActivationFunction = activationFunction ?? new Activations.Sigmoid();

        // Verify that the number of starting weights is correct
        var hiddenLayerWeightCount = inputCount * hiddenLayerNodes;
        if (!startingWeights.Length.Equals(hiddenLayerWeightCount + hiddenLayerNodes))
            throw new ArgumentException($"The number of starting weights must be {hiddenLayerWeightCount + hiddenLayerNodes}", nameof(startingWeights));

        // Verify that the number of starting biases is correct
        if (!startingBiases.Length.Equals(hiddenLayerNodes + 1))
            throw new ArgumentException($"The number of starting biases must be {hiddenLayerNodes + 1}", nameof(startingBiases));

        // Split the starting weights into the input layer and output layer weights
        var inputLayerWeights = startingWeights.Take(hiddenLayerWeightCount).ToArray();
        var outputLayerWeights = startingWeights.Skip(hiddenLayerWeightCount).Take(hiddenLayerNodes).ToArray();

        // Split the starting biases into the input layer and output layer biases
        var inputLayerBiases = startingBiases.Take(hiddenLayerNodes).ToArray();
        var outputLayerBias = startingBiases.Skip(hiddenLayerNodes).Single();

        // Construct the 2 layers
        _inputLayer = new InputLayer(inputCount, hiddenLayerNodes, inputLayerWeights, inputLayerBiases, this.ActivationFunction);
        _outputLayer = new OutputLayer(hiddenLayerNodes, outputLayerWeights, outputLayerBias, this.ActivationFunction);
    }

    public double Predict(double[] inputs)
    {
        // Predict the results of the input layer
        var inputLayerPrediction = _inputLayer.Predict(inputs);

        // Apply those results to the output layer (Feed Forward)
        return _outputLayer.Predict(inputLayerPrediction);
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

    public (double, IEnumerable<ScalarPrediction>) Test(IDictionary<double[], double> testSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value
        // Then calculate the error for the test set based on those predictions
        var predictions = Predict(testSet);
        return (predictions.CalculateMeanSquaredError(), predictions);
    }

    public bool Train(IDictionary<double[], double> trainingSet, 
        double convergenceThreshold = Constants.Training.DefaultConvergenceThreshold, 
        Action<int, Model, double>? callback = null)
    {
        // Verify that all training items have the correct number of inputs
        if (!trainingSet.All(item => item.Key.Count().Equals(_inputCount)))
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
            if (callback is not null && (this.TrainingIterations < 1000 || this.TrainingIterations % 1000 == 0))
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

    private Model Clone() => new Model(_inputLayer.Weights.Length, _outputLayer.Weights.Length, this.Weights, this.Biases)
    {
        TrainingConverged = this.TrainingConverged,
        TrainingIterations = this.TrainingIterations,
        ConvergenceThreshold = this.ConvergenceThreshold
    };
}
