using GD.Interfaces;
using Regression.Interfaces;
namespace GD;
internal class Program
{
    static List<(int, IPredictScalarValues, double)> _trainingResults = new();

    static void Main(string[] args)
    {
        var menuSelection = SelectMenuItem();
        IRegressionStrategy? model = GetModelStrategy(menuSelection);

        if (model is not null)
        {
            var trainedModel = model.Process(LogResult);

            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TrainedModel.json");
            var json = System.Text.Json.JsonSerializer.Serialize(trainedModel);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Model file written to " + filePath);

            TryIt(trainedModel);
        }
    }

    private static IRegressionStrategy? GetModelStrategy(int menuSelection)
    {
        IRegressionStrategy? model;
        if (menuSelection == 1)
            model = new Strategies.LinearRegressionStrategy(@".\Data\LinearData.csv");
        else if (menuSelection == 2)
            model = new Strategies.SimplePerceptronStrategy(@".\Data\house-votes-84.csv");
        else if (menuSelection == 3) // TODO: Load trained model
            model = new Strategies.SimplePerceptronStrategy(@".\Data\house-votes-84.csv", @".\Data\Params_HouseVotes_SimplePerceptron.json");
        else if (menuSelection == 4)
            model = new Strategies.MultilayerPerceptronStrategy(@".\Data\house-votes-84.csv");
        else if (menuSelection == 5) // TODO: Load trained model
            model = new Strategies.MultilayerPerceptronStrategy(@".\Data\house-votes-84.csv");
        else if (menuSelection == 6)
            model = null;
        else
            throw new NotImplementedException();

        return model;
    }

    private static int SelectMenuItem()
    {
        Console.WriteLine("1. Linear Regression - Distance Travelled");
        Console.WriteLine("2. Simple Perceptron - Train Congressional Votes");
        Console.WriteLine("3. Simple Perceptron - Predict Congressional Votes");
        Console.WriteLine("4. Multilayer Perceptron - Train Congressional Votes");
        Console.WriteLine("5. Multilayer Perceptron - Predict Congressional Votes");
        Console.WriteLine("6. Exit");

        var result = 0;
        while (result < 1 || result > 6)
        {
            Console.Write("Select an option: ");
            var option = Console.ReadLine();
            if (!int.TryParse(option, out result))
                Console.WriteLine("Invalid selection. Please try again.");
        }

        return result;
    }

    private static void TryIt(IPredictScalarValues trainedModel)
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

    static void LogResult(int iteration, IPredictScalarValues model, double mse)
    {
        _trainingResults.Add((iteration, model, mse));
        if (model.Weights.Length == 1)
            Console.WriteLine($"Iteration: {iteration} - M: {model.Weights[0]:0.000000000} - Bias: {model.Biases:0.000000000} - MSE: {mse:0.000000000}");
        else
            Console.WriteLine($"Iteration: {iteration} - MSE: {mse:0.000000000}");
    }

}