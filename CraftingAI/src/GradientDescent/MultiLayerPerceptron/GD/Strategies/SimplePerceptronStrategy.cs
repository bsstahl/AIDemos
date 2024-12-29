using GD.Interfaces;
using Regression.Interfaces;
using Regression.Extensions;

namespace GD.Strategies;

internal class SimplePerceptronStrategy : IRegressionStrategy
{
    private readonly string _dataPath;
    private readonly string? _parametersPath;
    private readonly bool _continueTraining;

    private readonly Random _random = new();

    public SimplePerceptronStrategy(string dataPath, string? parametersPath = null, bool continueTraining = true)
    {
        _dataPath = dataPath;
        _parametersPath = parametersPath;
        _continueTraining = continueTraining;

        if (!File.Exists(_dataPath))
            throw new FileNotFoundException($"Data file '{Path.GetFullPath(_dataPath)}' not found", dataPath);
        if (!string.IsNullOrWhiteSpace(parametersPath) && !File.Exists(parametersPath))
            throw new FileNotFoundException($"Parameters file '{Path.GetFullPath(parametersPath)}' not found", parametersPath);
    }

    public IPredictScalarValues Process(Action<int, IPredictScalarValues, double>? callback = null)
    {
        var data = GetVotingData(_dataPath);
        var (trainingSet, testSet) = data.Split(0.8f);

        var inputCount = trainingSet.First().Key.Length;
        var activationFunction = new Regression.Activations.Sigmoid();

        double[]? startingWeights = null;
        double? startingBias = null;
        bool isTrained = false;

        if (!string.IsNullOrWhiteSpace(_parametersPath))
        {
            // Load parameters from file
            var json = File.ReadAllText(_parametersPath);
            var parameters = System.Text.Json.JsonSerializer.Deserialize<Parameters>(json);
            startingWeights = parameters?.Weights;
            startingBias = parameters?.Biases[0];
            isTrained = true;
        }

        if (startingWeights is null || startingBias is null)
        {
            // Set parameters to start with random values near 0.5
            startingWeights = Enumerable.Range(0, inputCount)
            .Select(_ => _random.GetRandomDouble(0.45, 0.55))
            .ToArray();
            startingBias = _random.GetRandomDouble(-0.05, 0.05);
            isTrained = false;
        }

        var model = new Regression.LinearPerceptron.Model(trainingSet.First().Key.Length, startingWeights, startingBias.Value, activationFunction);

        // Skip training if the model is already trained
        if (!isTrained || _continueTraining)
        {
            isTrained = model.Train(trainingSet, callback: callback);
            if (isTrained)
            {
                // Write updated parameters to file
                var json = System.Text.Json.JsonSerializer.Serialize(new Parameters
                {
                    Weights = model.Weights,
                    Biases = [model.Bias]
                });

                var outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"TrainedModel_{DateTimeOffset.UtcNow.Ticks}.json");
                File.WriteAllText(outputFilePath, json);
                Console.WriteLine($"Parameters written to {outputFilePath}");
            }
        }

        var (Error, predictions) = model.Test(testSet);

        Console.WriteLine($"Test Error (of {predictions.Count()} predictions): {Error}");
        foreach (var prediction in predictions)
        {
            if (Math.Abs(prediction.Error ?? 1.0) > 0.25)
                Console.WriteLine($"Failed Prediction: Expected={prediction.Expected} Predicted={prediction.Predicted}");
        }

        return model;
    }

    private IDictionary<double[], double> GetVotingData(string dataPath)
    {
        var rawData = File.ReadAllLines(dataPath);
        return rawData
            .Select(line => line.Split(','))
            .Where(d => double.TryParse(d[0], out _))
            .Select(a => a.Select(v => double.Parse(v)).ToArray())
            .ToDictionary(p => p[1..17], p => p[0]);
    }

    private class Parameters
    {
        public double[] Weights { get; set; }
        public double[] Biases { get; set; }
    }
}
