using Beary.Documents.Interfaces;
using Beary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Documents.Test;

[ExcludeFromCodeCoverage]
public class Search_GetRelevantArticles_Should
{
    [Theory]
    [InlineData("SearchQuery1")]
    public async Task ReturnAValidSetOfResults(string searchKey)
    {
        var configElements = new List<KeyValuePair<string, string?>>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configElements)
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<Search>()
            .AddSingleton<IGetEmbeddings, Mocks.EmbeddingsClient>()
            .AddSingleton<IReadEmbeddingsSearchDocuments, Mocks.Library>()
            .AddSingleton<IReadContentSearchDocuments, Mocks.Library>()
            .BuildServiceProvider();

        var target = services.GetRequiredService<Search>();
        var results = await target.GetRelevantArticles(searchKey, 99999);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }
}