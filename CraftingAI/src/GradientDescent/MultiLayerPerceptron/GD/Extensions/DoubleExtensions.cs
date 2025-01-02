using System.Runtime.CompilerServices;

namespace GD.Extensions;

public static class DoubleExtensions
{
    public static double CalculateGradient(this double[] errors)
    {
        var sumOfErrors = errors.Sum();
        var scalingFactor = -2.0 / errors.Count();
        return scalingFactor * sumOfErrors;
    }

    public static double[] CalculateErrors(this double[] expected, double[] actual)
    {
        return expected.Length.Equals(actual.Length)
            ? expected.Select((e, i) => e - actual[i]).ToArray()
            : throw new ArgumentException("The expected and actual vectors must be the same length.");
    }

    public static double SigmoidDerivative(this double? value)
    {
        return value.HasValue
            ? value.Value.SigmoidDerivative()
            : throw new ArgumentNullException("You must supply a value to calculate the derivative.");
    }

    public static double SigmoidDerivative(this double value)
    {
        // Since this input is the result of the sigmoid function, the derivative
        // with respect to that output is just the output times 1 minus the output
        return value * (1.0 - value);
    }

    public static double[] Clean(this double?[] values)
    {
        return values.Where(v => v.HasValue).Select(v => v!.Value).ToArray();
    }

    public static double[] HadamardProduct(this double[] left, double[] right)
    {
        return left.Length.Equals(right.Length)
            ? left.Select((v, i) => v * right[i]).ToArray()
            : throw new ArgumentException("The vectors must be the same length.");
    }

    public static double[] ElementWiseAddition(this double[] vector, double addend)
    {
        return vector.Select(v => v + addend).ToArray();
    }
}
