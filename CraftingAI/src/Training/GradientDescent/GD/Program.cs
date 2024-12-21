using Regression;
using Regression.Extensions;

namespace GD;

internal class Program
{
    static List<TrainingResult> _trainingResults = new();

    static void Main(string[] args)
    {
        var data = GetLinearData(@".\Data\LinearData.csv");
        var (trainingSet, testSet) = data.Split(0.8f);
        var trainedModel = LinearModel.Train(trainingSet, callback: LogResult);
        var testError = trainedModel.Test(testSet);
        Console.WriteLine($"Test Error: {testError}");
        SaveResults(@"c:\s\temp\LinearTrainingResults.csv", _trainingResults);
    }

    static void LogResult(int iteration, LinearModel model, double mse)
    {
        var result = new TrainingResult(iteration, model.M, model.B, mse);
        if (iteration % 10000 == 0)
            Console.WriteLine($"Iteration: {iteration}, M: {model.M}, B: {model.B}, MSE: {mse}");
        _trainingResults.Add(result);
    }

    static IDictionary<double, double> GetLinearData(string filePath)
    {
        var rawData = File.ReadAllLines(filePath);
        return rawData.Select(line => line.Split(','))
            .ToDictionary(p => double.Parse(p[0]), p => double.Parse(p[1]));
    }

    static void SaveResults(string filePath, IEnumerable<TrainingResult> results)
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Iteration,M,B,Mean Squared Error");
        foreach (var result in results)
            csv.AppendLine($"{result.Iteration},{result.M},{result.B},{result.MeanSquaredError}");
        File.WriteAllText(filePath, csv.ToString());
    }

    private class TrainingResult(int iteration, double m, double b, double meanSquaredError)
    {
        public int Iteration { get; set; } = iteration;
        public double M { get; set; } = m;
        public double B { get; set; } = b;
        public double MeanSquaredError { get; set; } = meanSquaredError;
    }

}