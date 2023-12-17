using ADA2.Client.Entities;
using ADA2.Embeddings.Test.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using ADA2.Client.Extensions;
using Accord.Math;

namespace ADA2.Embeddings.Test;

[Collection("Embeddings")]
public class VectorMathTests
{
    private readonly ILogger _logger;
    private readonly EncodingEngine _encodingEngine;

    public VectorMathTests(ITestOutputHelper output)
    {
        var services = new ServiceCollection()
            .AddOpenAI(output)
            .BuildServiceProvider();
        _logger = services.GetRequiredService<ILogger<DistanceTests>>();
        _encodingEngine = services.GetRequiredService<EncodingEngine>();
    }

    [Theory]
    [InlineData("the queen")]
    [InlineData("la monarcha")]
    public async Task A_VectorMath_Addition(string testStatement)
    {
        var dictionary = new EmbeddingCollection(_encodingEngine)
        {
            "the monarch",
            "the woman",
            "la mujer",
            "la monarcha"
        };
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
        _logger.LogInformation("Test Results: {Distances}", distances);
    }


    [Theory]
    [InlineData("the queen")]
    [InlineData("the king")]
    public async Task B_VectorMath_Subtraction(string testStatement)
    {
        var dictionary = new EmbeddingCollection(_encodingEngine)
        {
            "the queen",
            "the woman",
            "the king",
            "the man"
        };
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
        _logger.LogInformation("Test Results: {Distances}", distances);
    }
}
