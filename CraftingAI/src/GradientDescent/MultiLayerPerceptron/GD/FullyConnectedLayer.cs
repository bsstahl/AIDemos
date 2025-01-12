﻿﻿﻿﻿﻿using GD.Interfaces;
using GD.Extensions;

namespace GD;

public class FullyConnectedLayer
{
    const int _maxIterations = 1000000;

    // -Max Double < Param < Max Double
    public double[] Weights { get; set; }
    public double[] Biases { get; set; }

    public bool TrainingConverged { get; set; }
    public int TrainingIterations { get; set; }
    // public double ConvergenceThreshold { get; set; }

    public IActivateNeurons ActivationFunction { get; set; }

    internal double[][] WeightsCollection => this.Weights.Chunk(this._inputCount).ToArray();

    private readonly int _inputCount;
    private readonly int _outputCount;

    public FullyConnectedLayer(int inputCount, int outputCount, double[] startingWeights, double[] startingBiases, IActivateNeurons? activationFunction = null)
    {
        _inputCount = inputCount;
        _outputCount = outputCount;
        this.Weights = startingWeights;
        this.Biases = startingBiases;
        this.ActivationFunction = activationFunction ?? new Activations.Sigmoid();
    }

    public double[] Predict(double[] inputs)
    {
        double[] outputs = new double[_outputCount];
        for (int i = 0; i < _outputCount; i++)
        {
            //int startingWeightIndex = _inputCount * i;
            //int endingWeightIndex = (i * _inputCount) + _inputCount;
            var nodeWeights = this.WeightsCollection[i];
            outputs[i] = FullyConnectedLayer.Predict(inputs, nodeWeights, Biases[i], this.ActivationFunction);
        }

        return outputs;
    }

    public IEnumerable<VectorPrediction> Predict(IDictionary<double[], double?[]> trainingSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value
        return trainingSet
            .Select(kvp => Predict(kvp.Key, kvp.Value))
            .ToArray();
    }

    public VectorPrediction Predict(double[] inputs, double?[] expectations) =>
        new VectorPrediction(_inputCount, _outputCount, inputs, Predict(inputs), expectations);

    public (double MeanSquaredError, IEnumerable<VectorPrediction> Predictions) Test(IDictionary<double[], double?[]> testSet)
    {
        // Make a prediction for each item in the set using the current model
        // The return value includes the input, the predicted value, and the expected value
        // Then calculate the error for the test set based on those predictions
        var predictions = this.Predict(testSet);
        return (predictions.CalculateMeanSquaredError(), predictions);
    }

    //internal IEnumerable<VectorPrediction> Train(IDictionary<double[], double?[]> trainingSet)
    //{
    //    // Get predictions for the training set
    //    var predictions = this.Predict(trainingSet);

    //    // Compute Error Gradients
    //    var deltaM = predictions.CalculateWeightsErrorGradient();
    //    var deltaB = predictions.CalculateBiasErrorGradient();

    //    this.UpdateParameters(deltaM, deltaB);

    //    // return a new set of predictions based on the updates to the model
    //    return this.Predict(trainingSet);
    //}

    internal bool UpdateParameters(double[] deltaM, double[] deltaB, double learningRate, double convergenceThreshold)
    {
        // Update Model Parameters
        for (int i = 0; i < this.Weights.Length; i++)
            this.Weights[i] -= learningRate * deltaM[i];

        for (int i = 0; i < this.Biases.Length; i++)
            this.Biases[i] -= learningRate * deltaB[i];

        // Determine if the model has converged
        return HasConverged(deltaM, deltaB, convergenceThreshold);
    }

    private bool HasConverged(double[] deltaM, double[] deltaB, double convergenceThreshold)
    {
        // Calculate RMS of gradients for more stable convergence check
        double rmsWeightGrad = Math.Sqrt(deltaM.Sum(x => x * x) / deltaM.Length);
        double rmsBiasGrad = Math.Sqrt(deltaB.Sum(x => x * x) / deltaB.Length);
        
        // Check if both RMS values are below threshold
        return rmsWeightGrad < convergenceThreshold && rmsBiasGrad < convergenceThreshold;
    }

    public void Save(string? filePath = null)
    {
        filePath ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"TrainedLinearPerceptron_{DateTimeOffset.UtcNow.Ticks}.json");
        var jsonModel = System.Text.Json.JsonSerializer.Serialize(this);
        File.WriteAllText(filePath, jsonModel);
        Console.WriteLine($"Model written to {filePath}");
    }

    private FullyConnectedLayer Clone() => new FullyConnectedLayer(_inputCount, _outputCount, this.Weights, this.Biases, this.ActivationFunction)
    {
        TrainingConverged = this.TrainingConverged,
        TrainingIterations = this.TrainingIterations
        // ConvergenceThreshold = this.ConvergenceThreshold
    };

    internal static double Predict(double[] inputs, double[] weights, double bias, IActivateNeurons activationFunction)
    {
        // Make a prediction for a single node
        return activationFunction.Activate(inputs.DotProduct(weights) + bias);
    }

}
