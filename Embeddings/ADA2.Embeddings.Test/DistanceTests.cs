using ADA2.Client.Entities;
using ADA2.Embeddings.Test.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ADA2.Embeddings.Test;

[Collection("Embeddings")]
public class DistanceTests
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;
    private readonly EncodingEngine _encodingEngine;

    public DistanceTests(ITestOutputHelper output)
    {
        var seriLogger = output.GetLogger(Serilog.Events.LogEventLevel.Information);
        _services = new ServiceCollection()
            .AddOpenAI(seriLogger)
            .BuildServiceProvider();
        _logger = _services.GetRequiredService<ILogger<DistanceTests>>();
        _encodingEngine = _services.GetRequiredService<EncodingEngine>();
    }

    [Theory]
    [InlineData(1, "I'm getting some RAM for my PC")]
    [InlineData(2, "I'm going to pull my boat with my Ram")]
    [InlineData(3, "I'm getting a ram and a ewe")]
    [InlineData(4, "I'm getting a new Ram")]
    public async Task A_Distance_Homonyms(int testId, string testStatement)
    {
        // Embeddings encode some of the context of the text, so
        // homonyms do not generally have the same embedding values.

        var dictionary = EmbeddingCollection.CreateFromText(_services, 
            "Goat", 
            "Memory", 
            "Truck");

        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);
    }

    [Theory]
    [InlineData(5, "He shoots and scores", "He kicked the ball")]
    [InlineData(6, "Boot to the dust", "He kicked the dirt")]
    [InlineData(7, "He kicked the bucket", "He died")]
    public async Task B_Distance_Idioms(int testId, string testStatement, string expected)
    {
        // Embeddings encode the idiomatic nature of certain expressions, so
        // that they will have similar values to a literal statement with the
        // same meaning.

        var dictionary = EmbeddingCollection.CreateFromText(_services,
            "He kicked the ball", 
            "He kicked the dirt",
            "He played with a pail",
            "He died");
    
        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);

        var actual = distances.OrderBy(d => d.Value).First().TargetEmbedding.Tag;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(8, "You're Early", "Actually early")]
    [InlineData(9, "Well, look who's on time", "Actually late")]
    public async Task C_Distance_Sarcasm(int testId, string testStatement, string expected)
    {
        // Embeddings encode the sarcastic nature of certain expressions, so
        // that they will have similar values to a sincere statement with the
        // same meaning.

        var dictionary = EmbeddingCollection.CreateFromText(_services,
            "Actually early",
            "Actually late");

        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);

        var actual = distances.OrderBy(d => d.Value).First().TargetEmbedding.Tag;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(10, "How old are you?", "What is your age?")]
    [InlineData(11, "¿Quieres una cerveza?", "Do you want a beer?")]
    public async Task D_Distance_Languages(int testId, string testStatement, string actual)
    {
        // Embeddings encode the meaning of expressions independent of the
        // language used, so they will have similar values to an equivalent statement
        // in another language, though not the same since language is also encoded.

        var dictionary = EmbeddingCollection.CreateFromText(_services,
            "What is your age?",
            "Do you want a beer?",
            "¿Cuántos años tienes?",
            "Hány éves vagy?",
            "Hvor gammel er du?");

        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);
    }

    [Theory]
    [InlineData(12, "discontent")]
    public async Task E_Distance_Antonym(int testId, string t1)
    {
        // The largest cosine distances are for words/phrases that are both
        // semantically and contextually opposite. Antonyms are not necessarily
        // maximally distant from each other since they share many common
        // traits and often are similar within the context.

        var dictionary = EmbeddingCollection.CreateFromText(_services, 
            "cellulose",
            "discomfort",
            "compliant",
            "contentment",
            "optimism",
            "peace",
            "audacious",
            "submissive",
            "bureaucracy",
            "liturgy",
            "liturgia",
            "horticulture");

        var distances = await _encodingEngine.GetDistances(_logger, dictionary, t1);
        _logger.LogInformation("Test {Id} Results: {Distances}", testId, distances);
    }
}