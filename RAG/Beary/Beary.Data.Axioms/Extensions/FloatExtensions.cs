namespace Beary.Data.Axioms.Extensions;

internal static class FloatExtensions
{
    public static double[] AsDoubleArray(this IEnumerable<float> values)
        => values.Select(e => Convert.ToDouble(e)).ToArray();
}
