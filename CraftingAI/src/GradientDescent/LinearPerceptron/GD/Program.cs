using GD.Extensions;

namespace GD;

internal class Program
{
    static List<(int, Model, double)> _trainingResults = new();
    static Random _random = new Random();

    static void Main(string[] args)
    {
        var data = GetVotingData(@".\Data\house-votes-84.csv");
        var (trainingSet, testSet) = data.Split(0.8f);

        var inputCount = trainingSet.First().Key.Length;
        var activationFunction = new Activations.Sigmoid();

        double[]? startingWeights = null;
        double? startingBias = null;
        bool isTrained = false;

        bool continueTraining = (args.Any() && args.Length > 1 && bool.TryParse(args[1], out var result)) ? result : false;

        // Load trained model if one is supplied in arguments
        if (args.Any() && !string.IsNullOrWhiteSpace(args[0]))
        {
            // Load model from file
            var jsonModel = File.ReadAllText(args[0]);
            var parameters = System.Text.Json.JsonSerializer.Deserialize<Model>(jsonModel);
            startingWeights = parameters?.Weights;
            startingBias = parameters?.Bias;
            isTrained = parameters?.TrainingConverged ?? false;
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

        Model model = new Model(trainingSet.First().Key.Length, startingWeights, startingBias.Value, activationFunction);

        // Skip training if the model is already trained unless the user wants to continue training
        if (!isTrained || continueTraining)
        {
            isTrained = model.Train(trainingSet, callback: LogResult);
            model.Save(); // Write updated parameters to file
        }

        var (Error, predictions) = model.Test(testSet);

        Console.WriteLine($"Test Error (of {predictions.Count()} predictions): {Error}");
        foreach (var prediction in predictions)
        {
            if (Math.Abs(prediction.Error ?? 1.0) > 0.25)
                Console.WriteLine($"Failed Prediction: Expected={prediction.Expected} Predicted={prediction.Predicted}");
        }

        TryIt(model);
    }

    private static void TryIt(Model trainedModel)
    {
        var done = false;
        do
        {
            var inputValues = GetInputValues(trainedModel.Weights.Length);
            done = inputValues is null;
            if (!done)
            {
                var result = trainedModel.Predict(inputValues!);
                Console.WriteLine($"Prediction: {result}");
            }
        } while (!done);
    }

    static double[]? GetInputValues(int length)
    {
        double[]? values = null;
        bool done = false;

        while (!done)
        {
            if (length == 1)
                Console.Write($"Enter input value: ");
            else if (length > 1)
                Console.WriteLine($"Enter {length} input values separated by commas: ");
            else
                throw new ArgumentOutOfRangeException(nameof(length));

            var inputString = Console.ReadLine() ?? string.Empty;
            values = inputString.Split(',')
                .Where(v => double.TryParse(v, out _))
                .Select(v => double.Parse(v)).ToArray()
                .ToArray();

            done = string.IsNullOrWhiteSpace(inputString) || values.Length == length;
        }

        return values?.Length.Equals(length) ?? false
            ? values
            : null;
    }

    static void LogResult(int iteration, Model model, double mse)
    {
        _trainingResults.Add((iteration, model, mse));
        Console.WriteLine($"Iteration: {iteration} - MSE: {mse:0.000000000}");
    }

    static private IDictionary<double[], double> GetVotingData(string dataPath)
    {
        var rawData = File.ReadAllLines(dataPath);
        return rawData
            .Select(line => line.Split(','))
            .Where(d => double.TryParse(d[0], out _))
            .Select(a => a.Select(v => double.Parse(v)).ToArray())
            .ToDictionary(p => p[1..17], p => p[0]);
    }

}