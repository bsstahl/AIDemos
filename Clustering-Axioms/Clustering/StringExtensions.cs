namespace Clustering;

public static class StringExtensions
{
    public static IEnumerable<string> GetRandom(this IEnumerable<string> source, float sampleFraction)
    {
        var rand = new Random();
        return source.Where(s => rand.NextDouble() <= sampleFraction);
    }
}
