namespace GD;

internal class Program
{
    static List<(int, Model, double)> _trainingResults = new();

    static void Main(string[] args)
    {
        var model = new Strategies.MultilayerPerceptronStrategy(@".\Data\house-votes-84.csv");

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
        Console.WriteLine($"Iteration: {iteration} - MSE: {mse:0.000000000}");
    }

}