using GD.Interfaces;
using Regression.Extensions;
using Regression.Interfaces;

namespace GD.Strategies;

public class LinearRegressionStrategy : IRegressionStrategy
{
    private string _dataPath;

    public LinearRegressionStrategy(string dataPath)
    {
        _dataPath = dataPath;
        if (!File.Exists(_dataPath))
            throw new FileNotFoundException($"Data file '{Path.GetFullPath(_dataPath)}' not found", dataPath);
    }

    public IPredictScalarValues Process(Action<int, IPredictScalarValues, double>? callback = null)
    {
        var data = GetLinearData(_dataPath);
        var (trainingSet, testSet) = data.Split(0.8f);

        var model = new Regression.Linear.Model();
        bool isTrained = model.Train(trainingSet, callback: callback);
        var testError = model.Test(testSet);

        Console.WriteLine($"Test Error: {testError}");

        return model;
    }

    static IDictionary<double[], double> GetLinearData(string filePath)
    {
        var rawData = File.ReadAllLines(filePath);
        return rawData
            .Select(line => line.Split(','))
            .ToDictionary(p => new double[] { double.Parse(p[0]) }, p => double.Parse(p[1]));
    }

}
