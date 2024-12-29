using GD.Extensions;

namespace GD;

internal class Program
{
    static List<(int, Model, double)> _trainingResults = new();

    static void Main(string[] args)
    {
        // TODO: If a previously trained model path is supplied, load it

        var data = GetData(@".\Data\LinearData.csv");
        var (trainingSet, testSet) = data.Split(0.8f);

        var model = new Model();
        bool isTrained = model.Train(trainingSet, callback: LogResult);

        // We don't log the individual test results here because they would have little meaning
        // Each value will always be "off" a bit since this is not a classification problem
        var testError = model.Test(testSet);

        // The goal of this process is to minimize this error so we can
        // make good predictions for the future
        Console.WriteLine($"Test Error: {testError}");

        if (isTrained)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(folderPath, $"TrainedModel_{DateTimeOffset.UtcNow.Ticks}.json");
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Model file written to " + filePath);
        }

        TryIt(model);
    }

    private static void TryIt(Model trainedModel)
    {
        var done = false;
        do
        {
            var inputValue = GetInputValue();
            done = inputValue is null;
            if (!done)
            {
                var result = trainedModel.Predict(inputValue!.Value);
                Console.WriteLine($"Prediction: Y({inputValue}) = {result}");
            }
        } while (!done);
    }

    static double? GetInputValue()
    {
        double? value = null;
        bool done = false;

        while (!done)
        {
            Console.Write($"Enter input value: ");

            var inputString = Console.ReadLine() ?? string.Empty;
            done = string.IsNullOrWhiteSpace(inputString) || double.TryParse(inputString, out var result);
            if (done && !string.IsNullOrWhiteSpace(inputString))
                value = double.Parse(inputString);
        }

        return value;
    }

    static void LogResult(int iteration, Model model, double mse)
    {
        _trainingResults.Add((iteration, model, mse));
        Console.WriteLine($"Iteration: {iteration} - M: {model.M:0.000000000} - Bias: {model.B:0.000000000} - MSE: {mse:0.000000000}");
    }

    static IDictionary<double, double> GetData(string filePath)
    {
        var rawData = File.ReadAllLines(filePath);
        return rawData
            .Select(line => line.Split(','))
            .ToDictionary(p => double.Parse(p[0]), p => double.Parse(p[1]));
    }

}