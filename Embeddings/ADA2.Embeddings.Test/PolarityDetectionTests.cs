using ADA2.Client.Entities;
using ADA2.Embeddings.Test.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ADA2.Embeddings.Test;

[Collection("Embeddings")]
public class PolarityDetectionTests
{
    private readonly IServiceProvider _services;
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    private readonly EncodingEngine _encodingEngine;

    public PolarityDetectionTests(ITestOutputHelper output)
    {
        var seriLogger = output.GetLogger(Serilog.Events.LogEventLevel.Verbose);
        _services = new ServiceCollection()
            .AddOpenAI(seriLogger)
            .BuildServiceProvider();
        _logger = _services.GetRequiredService<ILogger<DistanceTests>>();
        _encodingEngine = _services.GetRequiredService<EncodingEngine>();
    }

    [Theory]
    [InlineData(1, "Are you a c# developer?", "Yes", true)]
    [InlineData(2, "Do you have a pet chinchilla?", "I don't think so", false)]
    [InlineData(3, "Are dogs your favorite pets?", "I'm a canine lover", true)]
    [InlineData(4, "Do you like Javascript", "Nobody does", false)]
    public async Task A_Polarity_YesNo(int testId, string question, string answer, bool expected)
    {
        var dictionary = EmbeddingCollection.Create(_services);
        dictionary.Add("false", "negative");
        dictionary.Add("false", "no");
        dictionary.Add("true", "yes");
        dictionary.Add("true", "affirmative");

        var expression = $"'{answer}' as the answer to the question '{question}'.";

        var embeddingResults = await _encodingEngine.GetDistances(_logger, dictionary, expression);
        var primaryResult = bool.Parse(embeddingResults.First().TargetEmbedding.Tag);
        var secondaryResult = bool.Parse(embeddingResults.Skip(1).First().TargetEmbedding.Tag);

        var certainty = (primaryResult == secondaryResult)
            ? "Probably"
            : "Possibly";

        var result = $"{certainty} {primaryResult}";
        _logger.LogDebug("Result of test {Id}: {Result}", testId, result);

        Assert.Equal(expected, primaryResult);
    }
}