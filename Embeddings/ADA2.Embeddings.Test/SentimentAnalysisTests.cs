using ADA2.Client.Entities;
using ADA2.Embeddings.Test.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ADA2.Embeddings.Test;

[Collection("Embeddings")]
public class SentimentAnalysisTests
{
    private readonly IServiceProvider _services;
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    private readonly EncodingEngine _encodingEngine;

    public SentimentAnalysisTests(ITestOutputHelper output)
    {
        var seriLogger = output.GetLogger(Serilog.Events.LogEventLevel.Verbose);
        _services = new ServiceCollection()
            .AddOpenAI(seriLogger)
            .BuildServiceProvider();
        _logger = _services.GetRequiredService<ILogger<DistanceTests>>();
        _encodingEngine = _services.GetRequiredService<EncodingEngine>();
    }

    [Theory]
    [InlineData("I love speaking at great conferences")]
    [InlineData("I had to miss so many great conferences due to Covid")]
    [InlineData("I was scared out of my mind for my first live presentation")]
    [InlineData("I'm always impressed by the profound knowledge and enthusiasm showcased at technical conferences")]
    public async Task A_SentimentOf_Outliers(string expression)
    {
        string dictionaryFileName = "SentimentEmbeddings.json";
        float stdDev = 2.0810f; // # of standard deviations representing an "outlier"

        string dictionaryFilePath = Path.Combine(".\\Data", dictionaryFileName);

        var dictionary = EmbeddingCollection.CreateFromFile(_services, dictionaryFilePath);

        var expressionToLabel = $"Sentiment of: '{expression}'";
        var embeddingResult = await _encodingEngine.EmbedAsync(new[] { expressionToLabel });
        var embeddingValue = embeddingResult.IsError
            ? throw new InvalidOperationException("Cannot get embedding")
            : embeddingResult.Embeddings!.Single().Value;
        var tagResults = dictionary.GetNearestNeighbors(embeddingValue, stdDev, expression, expression);

        _logger.LogInformation("Sentiment: {Sentiments}", tagResults);
    }

    [Theory]
    [InlineData("I love speaking at great conferences")]
    [InlineData("I had to miss so many great conferences due to Covid")]
    [InlineData("I was scared out of my mind for my first live presentation")]
    [InlineData("I'm always impressed by the profound knowledge and enthusiasm showcased at technical conferences")]
    public async Task B_SentimentOf_2ndOrder(string expression)
    {
        string dictionaryFileName = "Emotions-wheel.json";
        float stdDev = float.MaxValue; // # of standard deviations representing an "outlier"

        string dictionaryFilePath = Path.Combine(".\\Data", dictionaryFileName);
        var dictionary = EmbeddingCollection.CreateFromFile(_services, dictionaryFilePath);

        var expressionToLabel = $"Sentiment of: '{expression}'";
        var embeddingResult = await _encodingEngine.EmbedAsync(new[] { expressionToLabel });
        var embeddingValue = embeddingResult.IsError
            ? throw new InvalidOperationException("Cannot get embedding")
            : embeddingResult.Embeddings!.Single().Value;
        var tagResults = dictionary.GetNearestNeighbors(embeddingValue, stdDev, expression, expressionToLabel);

        _logger.LogInformation("Sentiment: {Results}", tagResults);
    }
}
