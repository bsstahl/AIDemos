using Beary.ValueTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Data.AzureAISearch.Test;

[ExcludeFromCodeCoverage]
public class ContentReadRepository_Get_Should
{
    private readonly ServiceProvider _services;
    private readonly IConfiguration _config;

    public ContentReadRepository_Get_Should()
    {
        _config = new ConfigurationBuilder()
            .AddUserSecrets<ContentWriteRepository_Save_Should>()
            .Build();

        _services = new ServiceCollection()
            .BuildServiceProvider();
    }

    [Fact]
    public async Task SuccessfullyReadTheContentOfAnExistingArticle()
    {
        var searchServiceName = _config["SearchService:Name"];
        var apiKey = _config["SearchService:ApiKey"];

        var articleId = Guid.NewGuid();
        var content = string.Empty.GetRandom();
        var tokenCount = Int32.MaxValue.GetRandom();

        // Create the article
        var writeRepo = new Content.WriteRepository(searchServiceName, apiKey);
        await writeRepo.SaveAsync(
            Identifier.From(articleId),
            ArticleContent.From(content),
            TokenCount.From(tokenCount));

        // Read the article
        var readRepo = new Content.ReadRepository(searchServiceName, apiKey);
        var article = await readRepo.Get(Identifier.From(articleId));

        Assert.Equal(content, article!.Content!.Value);
    }


}