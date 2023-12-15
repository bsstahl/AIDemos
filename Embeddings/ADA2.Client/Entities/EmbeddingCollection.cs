using KdTree.Math;
using KdTree;
using System.Collections;
using ADA2.Client.Extensions;

namespace ADA2.Client.Entities;

public class EmbeddingCollection : IEnumerable<TextEmbedding>, IEnumerable
{
    private const int _dimensions = 1536;

    private readonly Dictionary<int, TextEmbedding> _dictionary;
    private readonly KdTree<float, int> _kdTree;
    private readonly FloatMath _typeMath;
    private readonly EncodingEngine _encodingEngine;

    /// <summary>
    /// Gets or sets the value associated with the specified embedded text
    /// </summary>
    /// <param name="embeddedText">The text that has been or will be embedded</param>
    /// <returns>A <see cref="T:TextEmbedding" /> object containing the text and possibly the embedding</returns>
    public TextEmbedding this[string embeddedText]
    {
        get
        {
            string embeddedText2 = embeddedText;
            return _dictionary.Single((d) => d.Value.EmbeddingText == embeddedText2).Value;
        }
    }

    /// <summary>
    /// Gets or sets the value associated with the specified index
    /// </summary>
    /// <param name="index">The cardinal index of the desired value</param>
    /// <returns>A <see cref="T:TextEmbedding" /> object containing the text and possibly the embedding</returns>
    public TextEmbedding this[int index] => _dictionary[index];

    /// <summary>
    /// Creates an empty collection
    /// </summary>
    public EmbeddingCollection(EncodingEngine encodingEngine)
    {
        _dictionary = new Dictionary<int, TextEmbedding>();
        _typeMath = new FloatMath();
        _kdTree = new KdTree<float, int>(1536, _typeMath);
        _encodingEngine = encodingEngine;
    }

    /// <summary>
    /// Adds an embedding to the collection
    /// </summary>
    /// <param name="embeddingText">The text containing the semantic meaning</param>
    /// <param name="embeddingValue">The point that represents the location in vector space</param>
    /// <returns>The index of the resulting object</returns>
    /// <remarks>The text doubles as the tag</remarks>
    public int Add(string embeddingText, float[]? embeddingValue = null)
    {
        return Add(embeddingText, embeddingText, embeddingValue);
    }

    /// <summary>
    /// Adds an embedding to the collection
    /// </summary>
    /// <param name="tag">A tag that can be used as a shortcut identifier</param>
    /// <param name="embeddingText">The text containing the semantic meaning</param>
    /// <param name="embeddingValue">The point that represents the location in vector space</param>
    /// <returns>The index of the resulting object</returns>
    public int Add(string tag, string embeddingText, float[]? embeddingValue = null)
    {
        return Add(_dictionary.Count, tag, embeddingText, embeddingValue);
    }

    private int Add(int index, string tag, string embeddingText, float[]? embeddingValue)
    {
        int num = embeddingValue?.Count() ?? 1536;
        switch (num)
        {
            case 0:
                num = 1536;
                embeddingValue = null;
                break;
            default:
                throw new ArgumentException(string.Format("{0} requires {1} dimensions but has {2}", "embeddingValue", 1536, num));
            case 1536:
                break;
        }
        _dictionary.Add(index, new TextEmbedding
        {
            Index = index,
            Tag = tag,
            EmbeddingText = embeddingText,
            EmbeddingValue = embeddingValue
        });
        if (embeddingValue != null)
        {
            _kdTree.Add(embeddingValue.ToArray(), index);
        }
        return index;
    }

    /// <summary>
    /// Using the supplied delegate, determine the embedding for each element in the collection that does not yet have one
    /// </summary>
    /// <param name="getEmbeddingsAsyncDelegate">A delegate that takes a string and asynchronously returns the embedding for that string</param>
    /// <param name="delay">A <see cref="T:System.TimeSpan" /> indicating how long to delay between individual embedding requests. Used to avoid flooding the service with requests.</param>
    public async Task PopulateEmbeddings(Func<IEnumerable<string>, Task<Dictionary<string, float[]>>> getEmbeddingsAsyncDelegate, TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(getEmbeddingsAsyncDelegate);
        delay ??= TimeSpan.FromSeconds(1.0);
        
        while (this.Any((e) => e.EmbeddingValue == null))
        {
            await PopulateEmbeddingsBatch(getEmbeddingsAsyncDelegate, delay.Value)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private async Task PopulateEmbeddingsBatch(Func<IEnumerable<string>, Task<Dictionary<string, float[]>>> getEmbeddingsAsyncDelegate, TimeSpan delay)
    {
        IEnumerable<TextEmbedding> requiredEmbeddings = this.Where((e) => e.EmbeddingValue == null).Take(16);
        IEnumerable<string> arg = requiredEmbeddings.Select((e) => e.EmbeddingText) ?? Array.Empty<string>();
        
        Dictionary<string, float[]> embeddingResults = await getEmbeddingsAsyncDelegate(arg)
            .ConfigureAwait(continueOnCapturedContext: false);
        
        foreach (var textEmbedding in requiredEmbeddings)
        {
            textEmbedding.EmbeddingValue = embeddingResults[textEmbedding.EmbeddingText];
            _kdTree.Add(textEmbedding.EmbeddingValue.ToArray(), textEmbedding.Index);
            await Task.Delay(delay).ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    public IEnumerable<VectorDistance> GetNearestNeighbors(float[] point, float standardDeviationsFromMean = 2f)
    {
        var nearestNeighbors = _kdTree
            .GetNearestNeighbours(point.ToArray(), Int32.MaxValue)
            .Select(n => _dictionary[n.Value])
            .Select(n => new VectorDistance()
            {
                SourcePoint = point,
                TargetEmbedding = new TextEmbedding()
                {
                    Index = n.Index,
                    Tag = n.Tag,
                    EmbeddingText = n.EmbeddingText,
                    EmbeddingValue = n.EmbeddingValue
                },
                Value = n.EmbeddingValue!.CosineDistance(point)
            });

        // Calculate statistics and return only nearest outliers
#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection
        // Requires at least 2 passes since the sumOfSquares can't be calculated without the mean
        var meanOfDistances = nearestNeighbors.Average(n => n.Value);
        var sumOfSquares = nearestNeighbors.Sum(x => (x.Value - meanOfDistances) * (x.Value - meanOfDistances));
        var stdDev = Convert.ToSingle(Math.Sqrt(sumOfSquares / (float)nearestNeighbors.Count()));

        var outlierDistance = (meanOfDistances - (standardDeviationsFromMean * stdDev));
        var result = nearestNeighbors
            .Where(n => n.TargetEmbedding.EmbeddingValue != point)
            .Where(n => (n.Value < outlierDistance))
            .OrderBy(n => n.Value)
            .AsEnumerable();
#pragma warning restore CA1851 // Possible multiple enumerations of 'IEnumerable' collection

        if (!result.Any()) // if there are no outliers, return the nearest neighbor
            result = nearestNeighbors.OrderBy(n => n.Value).Take(1);

        return result;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<TextEmbedding> GetEnumerator()
    {
        return _dictionary.Select((x) => x.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
