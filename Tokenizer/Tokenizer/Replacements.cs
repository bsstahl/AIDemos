using System.Globalization;
using System.Reflection;

namespace Tokenizer;

internal class Replacements
{
    IEnumerable<string[]>? _mergeableRanks;

    private IEnumerable<string[]> MergeableRanks
    {
        get
        {
            _mergeableRanks ??= LoadMergeableRanks("Tokenizer.data.cl100k_base.tiktoken");
            return _mergeableRanks;
        }
    }

    public IDictionary<int, byte[]> GetReplacementsByToken()
        => this.MergeableRanks.ToDictionary(
            splitLine => int.Parse(splitLine[1], CultureInfo.InvariantCulture),
            splitLine => Convert.FromBase64String(splitLine[0]));

    public IDictionary<byte[], int> GetReplacementsByText()
        => this.MergeableRanks.ToDictionary(
            splitLine => Convert.FromBase64String(splitLine[0]),
            splitLine => int.Parse(splitLine[1], CultureInfo.InvariantCulture), 
                new ByteArrayComparer());

    private static IEnumerable<string[]> LoadMergeableRanks(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var stream = assembly.GetManifestResourceStream(resourceName) ??
                     throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        reader.Close();
        reader.Dispose();
        stream.Close();
        stream.Dispose();

        var resource = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        return resource
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(line => line.Split(' ').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray());
    }


}
