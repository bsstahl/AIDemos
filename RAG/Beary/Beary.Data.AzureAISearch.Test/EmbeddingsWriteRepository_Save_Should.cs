using Beary.ValueTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Data.AzureAISearch.Test;

[ExcludeFromCodeCoverage]
public class EmbeddingsWriteRepository_Save_Should
{
    private readonly ServiceProvider _services;
    private readonly IConfiguration _config;

    public EmbeddingsWriteRepository_Save_Should()
    {
        _config = new ConfigurationBuilder()
            .AddUserSecrets<EmbeddingsWriteRepository_Save_Should>()
            .Build();

        _services = new ServiceCollection()
            .BuildServiceProvider();
    }

    [Fact]
    public async Task SucceedIfAllValuesAreSupplied()
    {
        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var url = $"http://www.example.com/{id}";
        var vector = new List<double> { 1.1, 2.2, 3.3 };

        var target = new Embeddings.WriteRepository(searchServiceName, apiKey);
        await target.SaveAsync(
            Identifier.From(id),
            ArticleContent.From(content),
            Location.From(url),
            Vector.From(vector));
    }

    [Fact]
    public async Task SucceedIfAllValuesButEmbeddingAreSupplied()
    {
        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var url = $"http://www.example.com/{id}";

        var target = new Embeddings.WriteRepository(searchServiceName, apiKey);
        await target.SaveAsync(
            Identifier.From(id),
            ArticleContent.From(content),
            Location.From(url));
    }

    [Fact]
    public async Task FailIfIdIsNotSupplied()
    {
        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var url = $"http://www.example.com/{id}";
        var vector = new List<double> { 1.1, 2.2, 3.3 };

        var target = new Embeddings.WriteRepository(searchServiceName, apiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(() => target.SaveAsync(
            null!,
            ArticleContent.From(content),
            Location.From(url),
            Vector.From(vector)));
    }

    [Fact]
    public async Task FailIfArticleContentIsNotSupplied()
    {
        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var url = $"http://www.example.com/{id}";
        var vector = new List<double> { 1.1, 2.2, 3.3 };

        var target = new Embeddings.WriteRepository(searchServiceName, apiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(() => target.SaveAsync(
            Identifier.From(id),
            null!,
            Location.From(url),
            Vector.From(vector)));
    }
}