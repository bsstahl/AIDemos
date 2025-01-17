using Beary.ValueTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Data.AzureAISearch.Test;

[ExcludeFromCodeCoverage, Collection("Content")]
public class ContentWriteRepository_Save_Should
{
    private readonly ServiceProvider _services;
    private readonly IConfiguration _config;

    public ContentWriteRepository_Save_Should()
    {
        _config = new ConfigurationBuilder()
            .AddUserSecrets<ContentWriteRepository_Save_Should>()
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
        var title = string.Empty.GetRandom();
        var tokenCount = Int32.MaxValue.GetRandom();

        var target = new Content.WriteRepository(searchServiceName, apiKey);
        await target.SaveAsync(
            Identifier.From(id),
            ArticleTitle.From(title),
            ArticleContent.From(content),
            TokenCount.From(tokenCount));
    }

    [Fact]
    public async Task NotFailIfTitleIsNotSupplied()
    {
        // This may be a temporary requirement

        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var id = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var tokenCount = Int32.MaxValue.GetRandom();

        var target = new Content.WriteRepository(searchServiceName, apiKey);
        await target.SaveAsync(
            Identifier.From(id),
            null!,
            ArticleContent.From(content),
            TokenCount.From(tokenCount));
    }

    [Fact]
    public async Task FailIfIdIsNotSupplied()
    {
        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var id = Guid.NewGuid();
        var title = string.Empty.GetRandom();
        var content = string.Empty.GetRandom();
        var tokenCount = Int32.MaxValue.GetRandom();

        var target = new Content.WriteRepository(searchServiceName, apiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(() => target.SaveAsync(
            null!,
            ArticleTitle.From(title),
            ArticleContent.From(content),
            TokenCount.From(tokenCount)));
    }

    [Fact]
    public async Task FailIfArticleContentIsNotSupplied()
    {
        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var id = Guid.NewGuid();
        var title = string.Empty.GetRandom();
        var content = string.Empty.GetRandom();
        var tokenCount = Int32.MaxValue.GetRandom();

        var target = new Content.WriteRepository(searchServiceName, apiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(() => target.SaveAsync(
            Identifier.From(id),
            ArticleTitle.From(title),
            null!,
            TokenCount.From(tokenCount)));
    }

    [Fact]
    public async Task FailIfTokenCountIsNotSupplied()
    {
        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var id = Guid.NewGuid();
        var title = string.Empty.GetRandom();
        var content = string.Empty.GetRandom();
        var tokenCount = Int32.MaxValue.GetRandom();

        var target = new Content.WriteRepository(searchServiceName, apiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(() => target.SaveAsync(
            Identifier.From(id),
            ArticleTitle.From(title),
            ArticleContent.From(content),
            null!));
    }
}