using GD.Extensions;
using GD.Interfaces;

namespace GD;

internal class Program
{
    private readonly List<(int, Model, double)> _trainingResults = new();

    private static void Main(string[] args)
    {
        const int hiddenLayerNodes = 3;
        const string dataFilePath = @".\Data\house-votes-84.csv";
        const float trainingSetPercentage = 0.8f;
        const double testTolerance = 0.25;

        var continueTraining = args.ContinueTraining();
        var parameterFilePath = args.ParameterPath();
        var activationFunction = new Activations.Sigmoid();

        var prog = new Program();
        prog.Execute(hiddenLayerNodes, dataFilePath, activationFunction, trainingSetPercentage, parameterFilePath, continueTraining, testTolerance);
    }

    private void Execute(int hiddenLayerNodes, string dataFilePath, IActivateNeurons activationFunction, float trainingSetPercentage, string parameterPath, bool continueTraining, double testTolerance)
    {
        List<(int, Model, double)> _trainingResults = new();

        var data = GetVotingData(dataFilePath);
        var (trainingSet, testSet) = data.Split(trainingSetPercentage);
        Model model = GetModel(trainingSet.First().Key.Length, hiddenLayerNodes, parameterPath, activationFunction);

        // Skip training if the model is already trained
        var isTrained = model.TrainingConverged;
        if (!isTrained || continueTraining)
        {
            isTrained = model.Train(trainingSet, callback: LogResult);
            if (isTrained)
                Console.WriteLine($"Parameters written to {model.Save()}");
        }

        var (error, predictions) = model.Test(testSet);

        var failedPredictions = predictions.Where(p => p.Failed(testTolerance));
        Console.WriteLine($"Failed {failedPredictions.Count()} of {predictions.Count()} predictions. Error: {error}");
        failedPredictions.ToList().ForEach(p => Console.WriteLine($"Expected: {p.Expected} Predicted: {p.Predicted}"));

        Console.WriteLine($"Model file written to {model.Save()}");

        TryIt(model);
    }

    private static Model GetModel(int inputCount, int hiddenLayerNodes, string parameterPath, IActivateNeurons activationFunction)
    {
        double[]? startingWeights = null;
        double[]? startingBiases = null;
        bool isTrained = false;

        // Load trained model if one is supplied in arguments
        if (!string.IsNullOrEmpty(parameterPath))
        {
            // Load model from file
            var jsonModel = File.ReadAllText(parameterPath);
            var parameters = System.Text.Json.JsonSerializer.Deserialize<Model>(jsonModel);
            startingWeights = parameters?.Weights;
            startingBiases = parameters?.Biases;
            isTrained = parameters?.TrainingConverged ?? false;
        }

        return new Model(inputCount, hiddenLayerNodes, startingWeights, startingBiases, activationFunction);
    }

    void TryIt(Model trainedModel)
    {
        var done = false;
        do
        {
            var inputValues = GetInputValues(trainedModel.InputCount);
            done = inputValues is null;
            if (!done)
            {
                var result = trainedModel.Predict(inputValues!);
                Console.WriteLine($"Prediction: {result}");
            }
        } while (!done);
    }

    double[]? GetInputValues(int length)
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

    static private IDictionary<double[], double> GetVotingData(string dataPath)
    {
        var rawData = File.ReadAllLines(dataPath);
        return rawData
            .Select(line => line.Split(','))
            .Where(d => double.TryParse(d[0], out _))
            .Select(a => a.Select(v => double.Parse(v)).ToArray())
            .ToDictionary(p => p[1..17], p => p[0]);
    }

    void LogResult(int iteration, Model model, double mse)
    {
        _trainingResults.Add((iteration, model, mse));
        Console.WriteLine($"Iteration: {iteration} - MSE: {mse:0.000000000}");
    }
}
