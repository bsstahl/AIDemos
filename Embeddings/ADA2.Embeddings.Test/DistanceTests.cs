using ADA2.Client.Entities;
using ADA2.Embeddings.Test.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ADA2.Embeddings.Test;

[Collection("Embeddings")]
public class DistanceTests
{
    private readonly ILogger _logger;
    private readonly EncodingEngine _encodingEngine;

    public DistanceTests(ITestOutputHelper output)
    {
        var services = new ServiceCollection()
            .AddOpenAI(output)
            .BuildServiceProvider();
        _logger = services.GetRequiredService<ILogger<DistanceTests>>();
        _encodingEngine = services.GetRequiredService<EncodingEngine>();
    }

    [Theory]
    [InlineData("I'm getting some RAM for my PC")]
    [InlineData("I'm going to pull my boat with my Ram")]
    [InlineData("I'm getting a ram and a ewe")]
    [InlineData("I'm getting a new Ram")]
    public async Task A_Distance_Homonyms(string testStatement)
    {
        var dictionary = new EmbeddingCollection(_encodingEngine) 
        { 
            "Goat",
            "Memory",
            "Truck"
        };
        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test Results: {Distances}", distances);
    }

    [Theory]
    [InlineData("He kicked the ball")]
    [InlineData("He kicked the dirt")]
    [InlineData("He kicked the bucket")]
    public async Task B_Distance_Idioms(string testStatement)
    {
        var dictionary = new EmbeddingCollection(_encodingEngine)
        {
            "He kicked the ball",
            "He kicked the dirt",
            "He kicked the bucket",
            "He died"
        };
        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test Results: {Distances}", distances);
    }

    [Theory]
    [InlineData("You're Early")]
    [InlineData("Well, look who's on time")]
    public async Task C_Distance_Sarcasm(string testStatement)
    {
        var dictionary = new EmbeddingCollection(_encodingEngine)
        {
            "Actually early",
            "Actually late"
        };
        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test Results: {Distances}", distances);
    }

    [Theory]
    [InlineData("How old are you?")]
    [InlineData("¿Quieres una cerveza?")]
    public async Task D_Distance_Languages(string testStatement)
    {
        var dictionary = new EmbeddingCollection(_encodingEngine)
        {
            "What is your age?",
            "Do you want a beer?",
            "¿Cuántos años tienes?",
            "Hány éves vagy?",
            "Hvor gammel er du?"
        };
        var distances = await _encodingEngine.GetDistances(_logger, dictionary, testStatement);
        _logger.LogInformation("Test Results: {Distances}", distances);
    }
}