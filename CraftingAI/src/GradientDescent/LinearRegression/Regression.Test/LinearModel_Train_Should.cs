using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Xunit.Abstractions;
using GD;

namespace Regression.Test;

[ExcludeFromCodeCoverage]
public class LinearModel_Train_Should
{
    private ITestOutputHelper _output;
    public LinearModel_Train_Should(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ReturnTheCorrectParameterValues()
    {
        var trainingSet = new Dictionary<double, double>
        {
            { 1.0, 2.0 },
            { 4.0, 5.0 },
            { 5.0, 6.0 },
            { 6.0, 7.0 },
            { 7.0, 8.0 },
            { 9.0, 10.0 },
            { 10.0, 11.0 }
        };

        var target = new Model();
        var isTrained = target.Train(trainingSet);
        _output.WriteLine(JsonSerializer.Serialize(target));

        // Model has been trained to at least 9 digits of precision
        var actualM = Math.Round(target.M, 9);
        var actualB = Math.Round(target.B, 9);

        Assert.True(target.TrainingConverged);
        Assert.Equal(1.0, actualM);
        Assert.Equal(1.0, actualB);
    }

    [Fact]
    public void CorrectlyPredictTheTestSet()
    {
        var trainingSet = new Dictionary<double, double>
        {
            { 1.0, 2.0 },
            { 4.0, 8.0 },
            { 5.0, 10.0 },
            { 6.0, 12.0 },
            { 7.0, 14.0 },
            { 9.0, 18.0 },
            { 10.0, 20.0 }
        };

        var testSet = new Dictionary<double, double>
        {
            { 2.0, 4.0 },
            { 3.0, 6.0 },
            { 8.0, 16.0 }
        };

        var target = new Model();
        var isTrained = target.Train(trainingSet);
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        testSet.ToList().ForEach(kvp =>
        {
            var prediction = target.Predict(kvp.Key);
            var actual = Math.Round(prediction, 9); // 9 digits of precision
            Assert.Equal(kvp.Value, actual);
        });
    }

    [Fact]
    public void CorrectlyPredictTheTestSetIfSlopeIsNegativeAndInterceptIsPositive()
    {
        var trainingSet = new Dictionary<double, double>
        {
            {-10.0,   37.0 },
            {  4.0,  -12.0 },
            {  6.0,  -19.0 },
            {  7.0,  -22.5 },
            {  9.0,  -29.5 }
        };

        var testSet = new Dictionary<double, double>
        {
            { -6.0,  23.0 },
            {  3.0,  -8.5 },
            {  8.0, -26.0 }
        };

        var target = new Model();
        var isTrained = target.Train(trainingSet);
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        testSet.ToList().ForEach(kvp =>
        {
            var prediction = target.Predict(kvp.Key);
            var actual = Math.Round(prediction, 9); // 9 digits of precision
            Assert.Equal(kvp.Value, actual);
        });
    }

    [Fact]
    public void CorrectlyPredictTheTestSetIfSlopeIsPositiveAndInterceptIsNegative()
    {
        var trainingSet = new Dictionary<double, double>
        {
            { -9.0,  -29.5 },
            { -4.0,  -17.0 },
            {  6.0,    8.0 },
            {  7.0,   10.5 },
            {  8.0,   13.0 }
        };

        var testSet = new Dictionary<double, double>
        {
            { -5.0, -19.5 },
            {  3.0,   0.5 },
            { 10.0,  18.0 }
        };

        var target = new Model();
        var isTrained = target.Train(trainingSet);
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        testSet.ToList().ForEach(kvp =>
        {
            var prediction = target.Predict(kvp.Key);
            var actual = Math.Round(prediction, 9); // 9 digits of precision
            Assert.Equal(kvp.Value, actual);
        });
    }

    [Fact]
    public void CorrectlyPredictTheTestSetIfSlopeAndInterceptAreNegative()
    {
        var trainingSet = new Dictionary<double, double>
        {
            { -9.0,   20.0 },
            { -4.0,    5.0 },
            {  6.0,  -25.0 },
            {  7.0,  -28.0 },
            {  8.0,  -31.0 }
        };

        var testSet = new Dictionary<double, double>
        {
            { -5.0,   8.0 },
            {  3.0,  -16.0 },
            { 10.0,  -37.0 }
        };

        var target = new Model();
        var isTrained = target.Train(trainingSet);
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        testSet.ToList().ForEach(kvp =>
        {
            var prediction = target.Predict(kvp.Key);
            var actual = Math.Round(prediction, 9); // 9 digits of precision
            Assert.Equal(kvp.Value, actual);
        });
    }

    [Fact]
    public void ReturnIntermediateResultsIfACallbackIsSupplied()
    {
        var trainingSet = new Dictionary<double, double>
        {
            { 1.0, 2.0 },
            { 4.0, 5.0 },
            { 5.0, 6.0 },
            { 6.0, 7.0 },
            { 7.0, 8.0 },
            { 9.0, 10.0 },
            { 10.0, 11.0 }
        };

        var intermediateIterationCounts = new List<int>();
        var intermediateModels = new List<Model>();
        var intermediateErrors = new List<double>();

        var target = new Model();
        var isTrained = target.Train(trainingSet, callback: (i, model, error) =>
        {
            intermediateIterationCounts.Add(i);
            intermediateModels.Add(model);
            intermediateErrors.Add(error);
        });

        _output.WriteLine(JsonSerializer.Serialize(target));
        _output.WriteLine($"Intermediate Iterations: Count={intermediateIterationCounts.Count()} First={intermediateIterationCounts.First()} Last={intermediateIterationCounts.Last()}");
        _output.WriteLine($"Intermediate Errors: Count={intermediateErrors.Count()} First={intermediateErrors.First()} Last={intermediateErrors.Last()}");
        _output.WriteLine($"Intermediate Models: Count={intermediateModels.Count()} Last={JsonSerializer.Serialize(intermediateModels.Last())}");

        Assert.True(target.TrainingConverged);
        Assert.True(intermediateIterationCounts.Count > 0);
    }

}
