namespace Regression.Extensions;

public static class DataSetExtensions
{
    public static (IDictionary<double[], double> TrainingSet, IDictionary<double[], double> TestSet) Split(this IDictionary<double, double> dataSet, float trainingPercentage)
    {
        var trainingSet = new Dictionary<double[], double>();
        var testSet = new Dictionary<double[], double>();

        var _random = new Random();
        foreach (var item in dataSet)
        {
            if (_random.NextDouble() < trainingPercentage)
                trainingSet.Add([item.Key], item.Value);
            else
                testSet.Add([item.Key], item.Value);
        }

        return (trainingSet, testSet);
    }
}
