﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using GD.Extensions;
using GD.Interfaces;
using System.Text.Json.Serialization;

namespace GD;

public class Model
{
    const int _maxIterations = 1000000;
    const double _learningRate = 0.01; // Increased learning rate for batch gradient descent

    private Random _random = new();

    // Combine weights from both layers in the same order they were split:
    // First the input layer weights, then the output layer weights
    public double[] Weights => _inputLayer.Weights.Concat(_outputLayer.Weights).ToArray();

    // Combine biases from both layers in the same order they were split:
    // First the input layer biases, then the output layer bias
    public double[] Biases => _inputLayer.Biases.Concat(_outputLayer.Biases).ToArray();

    public int InputCount => _inputCount;
    public int HiddenLayerNodes => _hiddenLayerNodes;

    public bool TrainingConverged { get; set; }
    public int TrainingIterations { get; set; }
    public double ConvergenceThreshold { get; set; }

    [JsonIgnore]
    public IActivateNeurons ActivationFunction { get; set; }

    private readonly int _inputCount;
    private readonly int _hiddenLayerNodes;

    // Model Layers
    private readonly FullyConnectedLayer _inputLayer;
    private readonly FullyConnectedLayer _outputLayer;

    [JsonConstructor]
    internal Model(int inputCount, int hiddenLayerNodes, double[] weights, double[] biases)
        : this(inputCount, hiddenLayerNodes, weights, biases, null)
    { }

    public Model(int inputCount, int hiddenLayerNodes, double[]? startingWeights = null, double[]? startingBiases = null, IActivateNeurons? activationFunction = null)
    {
        _inputCount = inputCount;
        _hiddenLayerNodes = hiddenLayerNodes;

        this.ActivationFunction = activationFunction ?? new Activations.Sigmoid();
        this.TrainingConverged = false;

        if (startingWeights is null)
        {
            // Xavier/Glorot initialization for better gradient flow
            var weightScale = Math.Sqrt(2.0 / (inputCount + hiddenLayerNodes));
            startingWeights = Enumerable.Range(0, (inputCount * hiddenLayerNodes) + hiddenLayerNodes)
                .Select(_ => _random.GetRandomDouble(-weightScale, weightScale))
                .ToArray();
            this.TrainingConverged = false;
        }

        if (startingBiases is null)
        {
            // Initialize biases to zero
            startingBiases = new double[hiddenLayerNodes + 1];
            this.TrainingConverged = false;
        }

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
        _inputLayer = new FullyConnectedLayer(inputCount, hiddenLayerNodes, inputLayerWeights, inputLayerBiases, this.ActivationFunction);
        _outputLayer = new FullyConnectedLayer(hiddenLayerNodes, 1, outputLayerWeights, [outputLayerBias], this.ActivationFunction);
    }

    public double Predict(double[] inputs)
    {
        // Predict the results of the input layer
        var inputLayerPrediction = _inputLayer.Predict(inputs);

        // Apply those results to the output layer (Feed Forward)
        return _outputLayer.Predict(inputLayerPrediction).Single();
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
        double previousError = double.MaxValue;
        while (!this.TrainingConverged && this.TrainingIterations < _maxIterations)
        {
            // Accumulate gradients over the entire batch
            var outputLayerWeightGradients = new double[_outputLayer.Weights.Length];
            var outputLayerBiasGradients = new double[_outputLayer.Biases.Length];
            var inputLayerWeightGradients = new double[_inputLayer.Weights.Length];
            var inputLayerBiasGradients = new double[_inputLayer.Biases.Length];

            foreach (var trainingItem in trainingSet)
            {
                // Forward pass
                var inputLayerActivations = _inputLayer.Predict(trainingItem.Key);
                var outputLayerExpectation = new double?[] { trainingItem.Value };
                var outputLayerActivation = _outputLayer.Predict(inputLayerActivations, outputLayerExpectation);

                // Calculate gradients
                var outputLayerDeltaM = outputLayerActivation.CalculateWeightsErrorGradient();
                var outputLayerDeltaB = outputLayerActivation.CalculateBiasesErrorGradient();
                var outputErrors = outputLayerActivation.WeightsErrors;
                var outputWeights = _outputLayer.WeightsCollection;
                var (hiddenErrors, hiddenGradients) = CalculateHiddenLayerErrors(outputErrors, outputWeights, inputLayerActivations, trainingItem.Key);

                // Accumulate gradients
                for (int i = 0; i < outputLayerWeightGradients.Length; i++)
                    outputLayerWeightGradients[i] += outputLayerDeltaM[i];
                for (int i = 0; i < outputLayerBiasGradients.Length; i++)
                    outputLayerBiasGradients[i] += outputLayerDeltaB[i];
                for (int i = 0; i < inputLayerWeightGradients.Length; i++)
                    inputLayerWeightGradients[i] += hiddenGradients[i];
                for (int i = 0; i < inputLayerBiasGradients.Length; i++)
                    inputLayerBiasGradients[i] += hiddenErrors[i];
            }

            // Scale gradients by batch size
            var batchSize = (double)trainingSet.Count;
            for (int i = 0; i < outputLayerWeightGradients.Length; i++)
                outputLayerWeightGradients[i] /= batchSize;
            for (int i = 0; i < outputLayerBiasGradients.Length; i++)
                outputLayerBiasGradients[i] /= batchSize;
            for (int i = 0; i < inputLayerWeightGradients.Length; i++)
                inputLayerWeightGradients[i] /= batchSize;
            for (int i = 0; i < inputLayerBiasGradients.Length; i++)
                inputLayerBiasGradients[i] /= batchSize;

            // Apply updates
            var outputLayerConverged = _outputLayer.UpdateParameters(outputLayerWeightGradients, outputLayerBiasGradients, _learningRate, this.ConvergenceThreshold);
            var inputLayerConverged = _inputLayer.UpdateParameters(inputLayerWeightGradients, inputLayerBiasGradients, _learningRate, this.ConvergenceThreshold);

            // Calculate error after full epoch
            var predictions = this.Predict(trainingSet);
            var testError = predictions.CalculateMeanSquaredError();

            // Output intermediate results periodically
            if (callback is not null && (this.TrainingIterations < 1000 || this.TrainingIterations % 1000 == 0))
                callback.Invoke(this.TrainingIterations, this.Clone(), testError);

            // Check convergence based on error improvement
            bool errorImproved = testError < previousError;
            bool errorConverged = this.TrainingIterations > 1000 && // Wait for at least 1000 iterations
                Math.Abs(previousError - testError) / previousError < this.ConvergenceThreshold && // Relative error change
                testError < 0.001; // Absolute error threshold
            bool gradientConverged = outputLayerConverged && inputLayerConverged;
            
            // Only converge if error is improving and meets thresholds
            this.TrainingConverged = errorImproved && errorConverged && gradientConverged;
            previousError = testError;
            
            this.TrainingIterations++;
        }

        return this.TrainingConverged;
    }

    private (double[] Errors, double[] Gradients) CalculateHiddenLayerErrors(
        double?[] outputErrors,
        double[][] outputWeights,
        double[] hiddenLayerActivations,
        double[] inputs)
    {
        var hiddenLayerSize = hiddenLayerActivations.Length;
        var inputLayerWeightCount = _inputCount * _hiddenLayerNodes;

        var errors = new double[hiddenLayerSize];
        var gradients = new double[inputLayerWeightCount];

        // For each hidden layer node
        for (int h = 0; h < hiddenLayerSize; h++)
        {
            // Get the weight connecting this hidden node to the output
            double outputWeight = outputWeights[0][h];

            // Get the derivative of this hidden node's activation
            double activationDerivative = hiddenLayerActivations[h].SigmoidDerivative();

            // Calculate error for this hidden node:
            // δh = -error * who * f'(zh)  (negated since we're using Expected - Predicted)
            // where error is raw output error, who is weight from hidden to output,
            // and f'(zh) is derivative of hidden node's activation
            errors[h] = -outputErrors[0]!.Value * outputWeight * activationDerivative;

            // Calculate gradients for each weight connected to this hidden node
            var startIndex = h * _inputCount;
            for (int i = 0; i < _inputCount; i++)
            {
                // The gradient for each weight is the hidden node's error times its input:
                // ∂E/∂wih = δh * xi
                // where δh is hidden node error and xi is input value
                gradients[startIndex + i] = errors[h] * inputs[i];
            }
        }

        return (errors, gradients);
    }

    public string Save(string? filePath = null)
    {
        filePath ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"TrainedModel_{DateTimeOffset.UtcNow.Ticks}.json");
        var json = System.Text.Json.JsonSerializer.Serialize(this);
        File.WriteAllText(filePath, json);
        return filePath;
    }

    private Model Clone() => new Model(_inputCount, _hiddenLayerNodes, this.Weights, this.Biases)
    {
        TrainingConverged = this.TrainingConverged,
        TrainingIterations = this.TrainingIterations,
        ConvergenceThreshold = this.ConvergenceThreshold
    };
}
