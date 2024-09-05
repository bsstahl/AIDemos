using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beary.Interfaces;
using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Extensions;
using Beary.ValueTypes;
using Beary.Embeddings.LocalServer.Extensions;

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

        foreach (var article in blogPosts)
        {
            var articleText = article.GetFullArticleText();

            var tokens = SharpToken.GptEncoding
                .GetEncodingForModel("gpt-4")
                .Encode(articleText);
            var tokenCount = TokenCount.From(tokens.Count);

            var embeddings = new List<Beary.Entities.ContentChunk>();
            foreach (var chunk in article.Chunks)
            {
                var embedding = await _embeddingsClient
                    .GetEmbedding(chunk.ChunkText, article.Id)
                    .ConfigureAwait(false);

                if (embedding is not null)
                    embeddings.Add(embedding);
            }

            await _writeRepo.SaveAsync(Identifier.From(article.Id),
                ArticleContent.From(articleText), tokenCount, embeddings);

        }
    }
}