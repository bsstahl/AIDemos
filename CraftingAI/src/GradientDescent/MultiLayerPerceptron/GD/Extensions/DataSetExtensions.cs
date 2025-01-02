namespace GD.Extensions;

public static class DataSetExtensions
{
    public static (IDictionary<double[], double> TrainingSet, IDictionary<double[], double> TestSet) Split(this IDictionary<double[], double> dataSet, float trainingPercentage)
    {
        var trainingSet = new Dictionary<double[], double>();
        var testSet = new Dictionary<double[], double>();

        var _random = new Random();
        foreach (var item in dataSet)
        {
            if (_random.NextDouble() < trainingPercentage)
                trainingSet.Add(item.Key, item.Value);
            else
                testSet.Add(item.Key, item.Value);
        }

        return (trainingSet, testSet);
    }

    public static IDictionary<double[], double?[]> AsTrainingSet(this double[][] inputData, double?[][] expected)
    {
        if (!inputData.Length.Equals(expected.Length))
            throw new ArgumentException("The number of input data items must match the number of expected data items", nameof(expected));

        var trainingSet = new Dictionary<double[], double?[]>();
        for (int i = 0; i < inputData.Length; i++)
            trainingSet.Add(inputData[i], expected[i]);

        return trainingSet;
    }
}
