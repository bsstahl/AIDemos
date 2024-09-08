using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beary.Interfaces;
using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Extensions;
using Beary.ValueTypes;
using Beary.Embeddings.LocalServer.Extensions;
using Bimp.BlogPostData;

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
            .AddSingleton<Program>()
            .AddHttpClient()
            .UseLocalServerEmbeddingsModel()
            .UseBearyWriteRepository()
            .UseAzureAIContentWriteRepo()
            .UseAzureAIEmbeddingsWriteRepo()
            .BuildServiceProvider();

        var program = services.GetRequiredService<Program>();
        await program.Execute();
    }

    private readonly BlogPostData.Repository _blogPostRepo;
    private readonly IWriteContent _writeRepo;
    private readonly IGetEmbeddings _embeddingsClient;

    public Program(BlogPostData.Repository blogPostRepo, IWriteContent writeRepo, IGetEmbeddings embeddingsClient)
    {
        _blogPostRepo = blogPostRepo;
        _writeRepo = writeRepo;
        _embeddingsClient = embeddingsClient;
    }

    async Task Execute()
    {
        var blogPosts = await _blogPostRepo.GetAllPosts();

        int i = 0;
        foreach (var article in blogPosts)
        {
            i++;
            var articleText = article.GetFullArticleText();

            var tokens = SharpToken.GptEncoding
                .GetEncodingForModel("gpt-4")
                .Encode(articleText);
            var tokenCount = TokenCount.From(tokens.Count);
            var chunkTextGroups = article.GetChunkGroups();

            var embeddings = new List<Beary.Entities.ContentChunk>();
            var embeddingResults = await _embeddingsClient
                .GetEmbeddings(chunkTextGroups, article.Id)
                .ConfigureAwait(false);
            embeddings.AddRange(embeddingResults);

            await _writeRepo.SaveAsync(Identifier.From(article.Id),
                ArticleContent.From(articleText), tokenCount, embeddings);

            Console.WriteLine($"Processed article {i} of {blogPosts.Count()}");
        }
    }
}