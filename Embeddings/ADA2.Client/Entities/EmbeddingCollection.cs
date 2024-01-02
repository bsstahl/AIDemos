using KdTree.Math;
using KdTree;
using System.Collections;
using ADA2.Client.Extensions;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ADA2.Client.Entities;

public class EmbeddingCollection : IEnumerable<TextEmbedding>, IEnumerable
{
    private const int _dimensions = 1536;

    private readonly Dictionary<int, TextEmbedding> _dictionary;
    private readonly KdTree<float, int> _kdTree;
    private readonly FloatMath _typeMath;
    private readonly EncodingEngine _encodingEngine;
    private readonly ILogger _logger;

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
    public EmbeddingCollection(ILogger<EmbeddingCollection> logger, EncodingEngine encodingEngine)
    {
        _logger = logger;
        _dictionary = new Dictionary<int, TextEmbedding>();
        _typeMath = new FloatMath();
        _kdTree = new KdTree<float, int>(1536, _typeMath);
        _encodingEngine = encodingEngine;
    }

    public void AddMany(IEnumerable<TextEmbedding>? embeddings)
    {
        (embeddings ?? Array.Empty<TextEmbedding>()).ToList()
            .ForEach(delegate (TextEmbedding e)
        {
            Add(e.Index, e.Tag, e.EmbeddingText, e.EmbeddingValue);
        });
    }

    public void AddMany(params string[] embeddingText)
        => (embeddingText ?? Array.Empty<string>()).ToList().ForEach(t => this.Add(t));

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

    public IEnumerable<VectorDistance> GetNearestNeighbors(float[] point, float standardDeviationsFromMean = 2f, string? sourcePointTag = null, string? sourcePointText = null)
    {
        var allNeighbors = GetAllNeighbors(point, sourcePointTag, sourcePointText);

        // Calculate statistics and return only nearest outliers
#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection
        // Requires at least 2 passes since the sumOfSquares can't be calculated without the mean
        var meanOfDistances = allNeighbors.Average(n => n.Value);
        var sumOfSquares = allNeighbors.Sum(x => (x.Value - meanOfDistances) * (x.Value - meanOfDistances));
        var stdDev = Convert.ToSingle(Math.Sqrt(sumOfSquares / (float)allNeighbors.Count()));

        var outlierDistance = (meanOfDistances - (standardDeviationsFromMean * stdDev));
        var result = allNeighbors
            .Where(n => n.TargetEmbedding.EmbeddingValue != point)
            .Where(n => (n.Value < outlierDistance))
            .OrderBy(n => n.Value)
            .AsEnumerable();
#pragma warning restore CA1851 // Possible multiple enumerations of 'IEnumerable' collection

        if (!result.Any()) // if there are no outliers, return the nearest neighbor
            result = allNeighbors.OrderBy(n => n.Value).Take(1);

        return result;
    }

    private IEnumerable<VectorDistance> GetAllNeighbors(float[] point, string? sourcePointTag, string? sourcePointText)
    {
        var nn = _kdTree.GetNearestNeighbours(point, Int32.MaxValue);
        var nn1 = nn.Select(n => _dictionary[n.Value]);
        var nearestNeighbors = nn1.Select(n => new VectorDistance()
        {
            SourceEmbedding = new TextEmbedding()
            {
                Tag = sourcePointTag ?? "Source",
                EmbeddingText = sourcePointText ?? "Source",
                EmbeddingValue = point
            },
            TargetEmbedding = new TextEmbedding()
            {
                Index = n.Index,
                Tag = n.Tag,
                EmbeddingText = n.EmbeddingText,
                EmbeddingValue = n.EmbeddingValue
            },
            Value = n.EmbeddingValue!.CosineDistance(point)
        });

        //var nearestNeighbors = _kdTree
        //    .GetNearestNeighbours(point, Int32.MaxValue)
        //    .Select(n => _dictionary[n.Value])
        //    .Select(n => new VectorDistance()
        //    {
        //        SourceEmbedding = new TextEmbedding()
        //        {
        //            Tag = sourcePointTag ?? "Source",
        //            EmbeddingText = sourcePointText ?? "Source",
        //            EmbeddingValue = point
        //        },
        //        TargetEmbedding = new TextEmbedding()
        //        {
        //            Index = n.Index,
        //            Tag = n.Tag,
        //            EmbeddingText = n.EmbeddingText,
        //            EmbeddingValue = n.EmbeddingValue
        //        },
        //        Value = n.EmbeddingValue!.CosineDistance(point)
        //    });

        _logger.LogDebug("Nearest Neighbors: {NearestNeighbors}", nearestNeighbors);
        return nearestNeighbors;
    }

    public static EmbeddingCollection Create(IServiceProvider services)
        => services.GetRequiredService<EmbeddingCollection>();

    /// <summary>
    /// Constructs a collection from the contents of the json file at the specified file path
    /// </summary>
    /// <param name="filePath">The full file path to be used to retrieve the embeddings</param>
    /// <returns>A new instance of <see cref="T:Carvana.Semantics.FreeText.Entities.EmbeddingCollection" /> containing the embeddings from the file</returns>
    public static EmbeddingCollection CreateFromFile(IServiceProvider services, string filePath)
    {
        string text = File.ReadAllText(filePath);
        return string.IsNullOrWhiteSpace(text)
            ? throw new InvalidOperationException("No data found in '" + filePath + "'")
            : CreateFromJson(services, text);
    }

    internal static EmbeddingCollection CreateFromJson(IServiceProvider services, string json)
    {
        var embeddings = JsonSerializer.Deserialize<IEnumerable<TextEmbedding>>(json);
        return CreateFromEmbeddings(services, embeddings);
    }

    public static EmbeddingCollection CreateFromEmbeddings(IServiceProvider services, IEnumerable<TextEmbedding>? embeddings)
    {
        var result = EmbeddingCollection.Create(services);
        result.AddMany(embeddings);
        return result;
    }

    public static EmbeddingCollection CreateFromText(IServiceProvider services, params string[] embeddingText)
    {
        var result = EmbeddingCollection.Create(services);
        result.AddMany(embeddingText);
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
