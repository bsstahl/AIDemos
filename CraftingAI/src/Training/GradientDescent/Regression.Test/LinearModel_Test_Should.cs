using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Xunit.Abstractions;

namespace Regression.Test;

[ExcludeFromCodeCoverage]
public class LinearModel_Test_Should
{
    private ITestOutputHelper _output;
    public LinearModel_Test_Should(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ReturnAnErrorBelowTheConvergenceThreshold()
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

        var target = LinearModel.Train(trainingSet);
        var actual = target.Test(testSet);

        _output.WriteLine($"Actual Error: {actual}");
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        Assert.True(actual < target.ConvergenceThreshold);
    }

    [Fact]
    public void ReturnAnErrorBelowTheConvergenceThresholdIfSlopeIsNegativeAndInterceptIsPositive()
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

        var target = LinearModel.Train(trainingSet);
        var actual = target.Test(testSet);

        _output.WriteLine($"Actual Error: {actual}");
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        Assert.True(actual < target.ConvergenceThreshold);
    }

    [Fact]
    public void ReturnAnErrorBelowTheConvergenceThresholdIfSlopeIsPositiveAndInterceptIsNegative()
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

        var target = LinearModel.Train(trainingSet);
        var actual = target.Test(testSet);

        _output.WriteLine($"Actual Error: {actual}");
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        Assert.True(actual < target.ConvergenceThreshold);
    }

    [Fact]
    public void ReturnAnErrorBelowTheConvergenceThresholdIfSlopeAndInterceptAreNegative()
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

        var target = LinearModel.Train(trainingSet);
        var actual = target.Test(testSet);

        _output.WriteLine($"Actual Error: {actual}");
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        Assert.True(actual < target.ConvergenceThreshold);
    }

    [Fact]
    public void ReturnAnErrorBelowTheSpecifiedConvergenceThreshold()
    {
        var trainingSet = new Dictionary<double, double>
        {
            { -9.0,   0.53815 },
            { -4.0,   0.37165 },
            {  6.0,   0.03865 },
            {  7.0,   0.00535 },
            {  8.0,  -0.02795 }
        };

        var testSet = new Dictionary<double, double>
        {
            { -5.0,  0.40495 },
            {  3.0,  0.13855 },
            { 10.0, -0.09455 }
        };

        var convergenceThreshold = 0.0001;
        var target = LinearModel.Train(trainingSet, convergenceThreshold);
        var actual = target.Test(testSet);

        _output.WriteLine($"Actual Error: {actual}");
        _output.WriteLine(JsonSerializer.Serialize(target));

        Assert.True(target.TrainingConverged);
        Assert.Equal(convergenceThreshold, target.ConvergenceThreshold);
        Assert.True(actual < convergenceThreshold);
    }

}
