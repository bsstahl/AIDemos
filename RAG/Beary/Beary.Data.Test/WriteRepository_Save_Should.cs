using Beary.Builders;
using Beary.Data.Interfaces;
using Beary.Data.Test.Extensions;
using Beary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Data.AzureAISearch.Test;

[ExcludeFromCodeCoverage, Collection("VectorSearch")]
public class WriteRepository_Save_Should
{
    private readonly ServiceProvider _services;
    private readonly IConfiguration _config;

    private readonly string _searchServiceName;
    private readonly string _apiKey;

    public WriteRepository_Save_Should()
    {
        _config = new ConfigurationBuilder()
            .AddUserSecrets<WriteRepository_Save_Should>()
            .Build();

        _searchServiceName = _config["SearchService:Name"];
        _apiKey = _config["SearchService:ApiKey"];

        _services = new ServiceCollection()
            .AddSingleton<IWriteContent, WriteRepository>()
            .AddSingleton<IWriteContentSearchDocuments, Content.WriteRepository>(c => new Content.WriteRepository(_searchServiceName, _apiKey))
            .AddSingleton<IWriteEmbeddingsSearchDocuments, Embeddings.WriteRepository>(c => new Embeddings.WriteRepository(_searchServiceName, _apiKey))
            .BuildServiceProvider();
    }

    [Fact]
    public async Task SucceedIfAllValuesAreSupplied()
    {
        var article = new ArticleBuilder()
            .UseRandomValues()
            .Build();

        var target = _services.GetRequiredService<Beary.Interfaces.IWriteContent>();
        await target.SaveAsync(article.Id!, article.Content!, article.TokenCount!, article.Chunks!);
    }

}