using Beary.Builders;
using Beary.ValueTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Serilog;
using Xunit.Abstractions;
using Beary.Data.AzureAISearch.Test.Extensions;

namespace Beary.Data.AzureAISearch.Test;

[ExcludeFromCodeCoverage, Collection("VectorSearch")]
public class EmbeddingsReadRepository_GetNearestNeighbors_Should
{
    private readonly ServiceProvider _services;
    private readonly IConfiguration _config;

    private readonly string _searchServiceName;
    private readonly string _apiKey;


    public EmbeddingsReadRepository_GetNearestNeighbors_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output).MinimumLevel.Verbose()
            .CreateLogger();

        _config = new ConfigurationBuilder()
            .AddUserSecrets<EmbeddingsReadRepository_GetNearestNeighbors_Should>()
            .Build();

        _searchServiceName = _config["SearchService:Name"];
        _apiKey = _config["SearchService:ApiKey"];

        _services = new ServiceCollection()
            .BuildServiceProvider();
    }

    [Fact]
    public async Task ReturnOneOrMoreResults()
    {
        int maxResultCount = 4;

        var contentChunk = new ContentChunkBuilder()
            .UseRandomValues(0)
            .Build();

        var id = contentChunk.Id;

        var writeRepo = new Embeddings.WriteRepository(_searchServiceName, _apiKey);
        var readRepo = new Embeddings.ReadRepository(_searchServiceName, _apiKey);

        var startingDocCount = await readRepo.GetDocumentCount();

        await writeRepo.SaveAsync(
            contentChunk.Id,
            contentChunk.Index,
            contentChunk.ChunkText,
            Identifier.From(Guid.NewGuid()),
            contentChunk.Embedding
        );

        var searchVector = contentChunk
            .Embedding!
            .Value
            .Select(v => v += 0.01f.GetRandom(-0.01f))
            .ToArray();

        Log.Logger.Information("Original Vector: {@vector}", contentChunk.Embedding!.Value);
        Log.Logger.Information("Search Vector: {@vector}", searchVector);

        var retryCount = await readRepo.WaitForDocumentIndexing(startingDocCount);
        Log.Logger.Information("Retry Count: {retryCount}", retryCount);

        var actual = await readRepo.GetNearestNeighbors(Vector.From(searchVector), ResultCount.From(maxResultCount));

        actual.ToList().ForEach(d => Log.Logger.Information("{@document}", d));

        var firstResult = actual.First();
        Assert.Equal(id.Value, actual.First().Id);
    }


}