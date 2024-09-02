using Beary.ValueTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Data.AzureAISearch.Test;

[ExcludeFromCodeCoverage]
public class EmbeddingsReadRepository_GetNearestNeighbors_Should
{
    private readonly ServiceProvider _services;
    private readonly IConfiguration _config;

    private readonly string _searchServiceName;
    private readonly string _apiKey;


    public EmbeddingsReadRepository_GetNearestNeighbors_Should()
    {
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
        // TODO: Add content to search for 

        int maxResultCount = 4;
        var vector = new List<Single> 
        {
            0.86137690f,
            0.97457104f,
            0.55426147f
        };

        var target = new Embeddings.ReadRepository(_searchServiceName, _apiKey);
        var actual = await target.GetNearestNeighbors(Vector.From(vector), ResultCount.From(maxResultCount));

        Assert.True(actual.Any());
    }


}