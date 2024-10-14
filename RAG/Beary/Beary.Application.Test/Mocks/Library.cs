using Beary.Application.Interfaces;
using Beary.Entities;
using Beary.Interfaces;
using Beary.ValueTypes;

namespace Beary.Application.Test.Mocks;

[ExcludeFromCodeCoverage]
internal class Library : IReadSourceDocuments, IReadEmbeddingsSearchDocuments, IReadContentSearchDocuments
{
    private readonly IGetEmbeddings _embeddingsClient;
    private readonly Article[] _articles;

    public Library(IGetEmbeddings embeddingsClient)
    {
        _embeddingsClient = embeddingsClient;
        _articles = Load();
    }

    public Task<bool> ArticleExists(string articleId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Document>> GetAllDocuments()
    {
        return Task.FromResult(_articles.Select(a => new Document()
        {
            Id = a.Id.Value,
            Title = a.Title.Value,
            FullText = a.Content.Value,
            ContentChunks = a.Chunks.Select(c => c.ChunkText.Value)
        }));
    }

    public Task<IEnumerable<SearchResult>> GetAllEmbeddings()
    {
        throw new NotImplementedException();
    }

    public Task<Article> GetArticle(string articleId)
    {
        return Task.FromResult(_articles.First(a => a.Id.Value == articleId));
    }

    public Task<long> GetDocumentCount()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SearchResult>> GetNearestNeighbors(IEnumerable<float> queryVector, int numberOfNeighbors)
    {
        // Return the first chunk of article 1 and the 2nd of article 2
        var article1 = _articles[0];
        var article2 = _articles[1];

        var results = new List<SearchResult>()
        {
            new SearchResult()
            {
                Id = article1.Chunks.First().Id.Value,
                ItemId = article1.Id.Value,
                Content = article1.Chunks.First().ChunkText.Value,
                Score = 0.93f,
                Embedding = article1.Chunks.First().Embedding.Value,
                ElementIndex = 0
            },
            new SearchResult()
            {
                Id = article2.Chunks.Skip(1).First().Id.Value,
                ItemId = article2.Id.Value,
                Content = article2.Chunks.Skip(1).First().ChunkText.Value,
                Score = 0.89f,
                Embedding = article2.Chunks.Skip(1).First().Embedding.Value,
                ElementIndex = 1
            }
        };

        return Task.FromResult(results.AsEnumerable());
    }

    public Task SaveAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding)
    {
        throw new NotImplementedException();
    }

    private Article[] Load()
    {
        var article1Id = "c7f2f5d3-c0b8-400d-9692-b4594223924c";
        var article1Title = $"Title of article {article1Id}";

        var article2Id = "6a05260c-66ca-4ed0-b2ea-5a7b7790109c";
        var article2Title = $"Title of article {article2Id}";

        var article1EmbeddingTask = _embeddingsClient
            .GetEmbeddings(new[] { "This is the content", $"of article {article1Id}" }, article1Id);

        var article2EmbeddingTask = _embeddingsClient
            .GetEmbeddings(new[] { "This is some content", $"from article {article2Id}" }, article2Id);

        Task.WaitAll(article1EmbeddingTask, article2EmbeddingTask);

        var article1Embeddings = article1EmbeddingTask.Result;
        var article2Embeddings = article2EmbeddingTask.Result;

        var article1Text = string.Join(" ", article1Embeddings.Select(a => a.ChunkText));
        var article2Text = string.Join(" ", article2Embeddings.Select(a => a.ChunkText));

        return new List<Article>()
        {
            new Article(
                article1Id,
                article1Title,
                article1Text,
                10,
                article1Embeddings
            ),
            new Article(
                article2Id,
                article2Title,
                article2Text,
                11,
                article2Embeddings
            )
        }.ToArray();
    }
}
