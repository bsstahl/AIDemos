using ADA2.Client.Entities;
using ADA2.Embeddings.Test.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using ADA2.Client.Extensions;
using Accord.Math;

namespace ADA2.Embeddings.Test;

[Collection("Embeddings")]
public class MathTests
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;
    private readonly EncodingEngine _encodingEngine;

    public MathTests(ITestOutputHelper output)
    {
        var seriLogger = output.GetLogger(Serilog.Events.LogEventLevel.Information);
        _services = new ServiceCollection()
            .AddOpenAI(seriLogger)
            .BuildServiceProvider();
        _logger = _services.GetRequiredService<ILogger<DistanceTests>>();
        _encodingEngine = _services.GetRequiredService<EncodingEngine>();
    }

    [Theory]
    [InlineData(1, "the queen")]
    [InlineData(2, "la monarcha")]
    public async Task A_VectorMath_Addition(int testId, string testStatement)
    {
        // Adding 2 vectors together produces a new vector that is very close to the semantic
        // meaning of the "sum" of the 2 original meanings.

        // Monarch + Woman is very close to Queen
        // Monarcha + Mujer is very close to Reina

        var dictionary = EmbeddingCollection.CreateFromText(_services,
            "the monarch",
            "the woman",
            "la mujer",
            "la monarcha");
        await dictionary.PopulateEmbeddings(_encodingEngine.getEmbeddingsDelegate, TimeSpan.FromSeconds(0));

        var monarch = dictionary["the monarch"];
        var woman = dictionary["the woman"];
        var mujer = dictionary["la mujer"];
        var monarcha = dictionary["la monarcha"];

        var monarchPlusWoman = monarch.EmbeddingValue!.Sum(woman.EmbeddingValue!).Normalize();
        var monarchaPlusMujer = monarcha.EmbeddingValue!.Sum(mujer.EmbeddingValue!).Normalize();

        dictionary.Add("monarch+woman", monarchPlusWoman);
        dictionary.Add("monarcha+mujer", monarchaPlusMujer);

        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);
    }


    [Theory]
    [InlineData(3, "the queen")]
    [InlineData(4, "the king")]
    public async Task B_VectorMath_Subtraction(int testId, string testStatement)
    {
        // Subtracting 1 vector from another produces a new vector that is very close to the semantic
        // meaning of the "subtraction" of the 2 original meanings.

        // queen-woman+man is very close to king
        // king-man+woman is very close to queen

        var dictionary = EmbeddingCollection.CreateFromText(_services,
            "the queen",
            "the woman",
            "the king",
            "the man");
        await dictionary.PopulateEmbeddings(_encodingEngine.getEmbeddingsDelegate, TimeSpan.FromSeconds(0));

        var queen = dictionary["the queen"];
        var woman = dictionary["the woman"];
        var king = dictionary["the king"];
        var man = dictionary["the man"];

        // Be sure to only normalize once here
        // If you use intermediate steps, don't normalize until the very end

        var queenMinusWomanPlusMan = queen.EmbeddingValue!
            .Difference(woman.EmbeddingValue!)
            .Add(man.EmbeddingValue!)
            .Normalize();

        var kingMinusManPlusWorman = king.EmbeddingValue!
            .Difference(man.EmbeddingValue!)
            .Add(woman.EmbeddingValue!)
            .Normalize();

        dictionary.Add("queen-woman+man", queenMinusWomanPlusMan);
        dictionary.Add("king-man+woman", kingMinusManPlusWorman);

        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);
    }

    [Theory]
    [InlineData(5, "la reina")]
    [InlineData(6, "el rey")]
    [InlineData(7, "the queen")]
    [InlineData(8, "the king")]
    public async Task C_VectorMath_Negation(int testId, string testStatement)
    {
        // The distance from a vector to the negation of that vector is always ~2.0

        var dictionary = EmbeddingCollection.CreateFromText(_services,
            "the king",
            "the queen",
            "la reina",
            "el rey");
        await dictionary.PopulateEmbeddings(_encodingEngine.getEmbeddingsDelegate, TimeSpan.FromSeconds(0));

        var values = dictionary.ToList();
        foreach (var embedding in values)
        {
            var negation = embedding.EmbeddingValue!.Negate();
            dictionary.Add($"-{embedding.Tag}", negation);
        }

        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);
    }

    [Theory]
    [InlineData(9, "la reina", "the queen")]
    [InlineData(10, "el rey", "the king")]
    public async Task D_VectorMath_LanguageSubtraction(int testId, string t1, string t2)
    {
        // Nearly all dimensions are changed significantly when translating between English and Spanish
        // Meaning we can't identify specific dimensions that encode language

        var dictionary = EmbeddingCollection.CreateFromText(_services, t1, t2);
        await dictionary.PopulateEmbeddings(_encodingEngine.getEmbeddingsDelegate, TimeSpan.FromSeconds(0));

        var v1 = dictionary[t1];
        var v2 = dictionary[t2];
        var difference = v1.EmbeddingValue!
            .Difference(v2.EmbeddingValue!)
            .Normalize();

        _logger.LogTrace("Test Results: {Vector}", difference);

        var sig = 0.001f;
        _logger.LogInformation("Count for Test {Id} where |value| < {Sig}: {Count}", testId, sig, difference.Count(v => Math.Abs(v) < sig));
    }

    [Theory]
    [InlineData(15, "Duckling", "Duck")]
    public async Task F_Distance_Analogical(int testId, string t1, string t2)
    {
        // The vector difference between puppy and dog does not capture
        // the relationship between them in a repeatable way so we cannot
        // supply that difference to other analogous relationships

        var dictionary = EmbeddingCollection.CreateFromText(_services,
            "Puppy",
            "Dog",
            t1,
            t2
            );
        await dictionary.PopulateEmbeddings(_encodingEngine.getEmbeddingsDelegate, TimeSpan.FromSeconds(0));

        var puppy = dictionary["Puppy"];
        var dog = dictionary["Dog"];

        var v1 = dictionary[t1];
        var v2 = dictionary[t2];

        var diff = dog.EmbeddingValue.Difference(puppy.EmbeddingValue);
        var result = v1.EmbeddingValue.Sum(diff).Normalize();

        dictionary.Add($"dog-puppy+{t1}", result);

        var distances = await _encodingEngine.GetDistances(_logger, dictionary, t2);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);
    }

}
