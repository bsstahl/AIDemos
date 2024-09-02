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

    private readonly string _searchServiceName;
    private readonly string _apiKey;

    public EmbeddingsWriteRepository_Save_Should()
    {
        _config = new ConfigurationBuilder()
            .AddUserSecrets<EmbeddingsWriteRepository_Save_Should>()
            .Build();

        _searchServiceName = _config["SearchService:Name"];
        _apiKey = _config["SearchService:ApiKey"];

        _services = new ServiceCollection()
            .BuildServiceProvider();
    }

    [Fact]
    public async Task SucceedIfAllValuesAreSupplied()
    {
        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var articleId = Guid.NewGuid();
        var vector = new List<Single> { 1.1f, 2.2f, 3.3f };

        var target = new Embeddings.WriteRepository(_searchServiceName, _apiKey);
        await target.SaveAsync(
            Identifier.From(id),
            ElementIndex.From(0),
            ArticleContent.From(content),
            Identifier.From(articleId),
            Vector.From(vector));
    }

    [Fact]
    public async Task SucceedIfAllValuesButEmbeddingAreSupplied()
    {
        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var articleId = Guid.NewGuid();

        var target = new Embeddings.WriteRepository(_searchServiceName, _apiKey);
        await target.SaveAsync(
            Identifier.From(id),
            ElementIndex.From(0),
            ArticleContent.From(content),
            Identifier.From(articleId));
    }

    [Fact]
    public async Task FailIfIdIsNotSupplied()
    {
        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var articleId = Guid.NewGuid();
        var vector = new List<Single> { 1.1f, 2.2f, 3.3f };

        var target = new Embeddings.WriteRepository(_searchServiceName, _apiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(() => target.SaveAsync(
            null!,
            ElementIndex.From(0),
            ArticleContent.From(content),
            Identifier.From(articleId),
            Vector.From(vector)));
    }

    [Fact]
    public async Task FailIfElementIndexIsNotSupplied()
    {
        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var articleId = Guid.NewGuid();
        var vector = new List<Single> { 1.1f, 2.2f, 3.3f };

        var target = new Embeddings.WriteRepository(_searchServiceName, _apiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(() => target.SaveAsync(
            Identifier.From(id),
            null!,
            ArticleContent.From(content),
            Identifier.From(articleId),
            Vector.From(vector)));
    }

    [Fact]
    public async Task FailIfArticleContentIsNotSupplied()
    {
        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var articleId = Guid.NewGuid();
        var vector = new List<Single> { 1.1f, 2.2f, 3.3f };

        var target = new Embeddings.WriteRepository(_searchServiceName, _apiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(() => target.SaveAsync(
            Identifier.From(id),
            ElementIndex.From(0),
            null!,
            Identifier.From(articleId),
            Vector.From(vector)));
    }
}