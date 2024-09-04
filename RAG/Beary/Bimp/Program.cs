using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beary.Interfaces;
using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Extensions;
using Beary.ValueTypes;
using Bimp.Extensions;

namespace Bimp;

/// <summary>
/// Beary Import - Grab data from the blog post repository and import it into Azure Search.
/// </summary>
internal class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Bimp.Program>()
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<BlogPostData.Repository>()
            .UseBearyWriteRepository()
            .UseAzureAIContentWriteRepo()
            .UseAzureAIEmbeddingsWriteRepo()
            .BuildServiceProvider();

        var program = new Program();
        await program.Execute(services);
    }

    async Task Execute(IServiceProvider services)
    {
        var blogPostRepo = services.GetRequiredService<BlogPostData.Repository>();
        var blogPosts = await blogPostRepo.GetAllPosts();

        var writeRepo = services.GetRequiredService<IWriteContent>();

        foreach (var article in blogPosts)
        {
            var articleText = article.GetFullArticleText();

            var tokens = SharpToken.GptEncoding
                .GetEncodingForModel("gpt-4")
                .Encode(articleText);
            var tokenCount = TokenCount.From(tokens.Count);

            var embeddings = await article.Chunks.Select(c => c.ChunkText).GetEmbeddings(article.Id);

            await writeRepo.SaveAsync(Identifier.From(article.Id),
                ArticleContent.From(articleText), tokenCount, embeddings);

        }
    }
}