using Beary.Application.Extensions;
using Beary.Application.Interfaces;
using Beary.Entities;
using Beary.Interfaces;
using Beary.ValueTypes;

namespace Beary.Application;

public class Search
{
    const int defaultArticleCount = 10;

    private readonly IGetEmbeddings _embeddingsClient;
    private readonly IReadEmbeddingsSearchDocuments _embeddingsReadRepo;
    private readonly IReadContentSearchDocuments _contentReadRepo;

    public Search(IGetEmbeddings embeddingsClient, IReadEmbeddingsSearchDocuments embeddingsReadRepo, IReadContentSearchDocuments contentReadRepo)
    {
        _embeddingsClient = embeddingsClient;
        _embeddingsReadRepo = embeddingsReadRepo;
        _contentReadRepo = contentReadRepo;
    }

    // Get the Embedding for the text and then call the next overload
    public async Task<IEnumerable<Document>> GetRelevantArticles(string text, int maxTokenCount)
    {
        var maxTokens = TokenCount.From(maxTokenCount);

        IEnumerable<Beary.Entities.Article> articles = [];
        var requestId = Guid.NewGuid().ToString();
        var embeddedText = await _embeddingsClient.GetEmbedding(text, requestId);
        
        IEnumerable<Entities.Document> documents;
        if (embeddedText.IsPopulated())
            documents = await this.GetRelevantDocuments(embeddedText!.Embedding!, maxTokens);
        else
        {
            // TODO: Fail-over to keyword search if configured to do so
            throw new NotImplementedException();
        }

        return documents;
    }

    // Orchestrates the underlying repositories and contains the logic to determine
    // which articles to return based on vector search results.
    // Process steps:
    // 1. Get the search results from the Azure AI Search repository.
    // 2. Determine the n most relevant, unique articles (sometimes the same article
    //    will have multiple search results).
    // 3. Get the articles from the Content repository.
    private async Task<IEnumerable<Entities.Document>> GetRelevantDocuments(Vector embedding, TokenCount maxTokenCount)
    {
        var results = new List<Entities.Document>();

        var nearestNeighbors = await _embeddingsReadRepo
            .GetNearestNeighbors(embedding.Value, defaultArticleCount);

        int i = 0;
        int tokenCount = 0;
        var triedArticleIds = new List<Identifier>();
        var embeddingResults = nearestNeighbors.ToArray();
        while (tokenCount < maxTokenCount.Value && i < nearestNeighbors.Count())
        {
            // TODO: Ignore the article if the results are below a minimum score
            var articleId = Identifier.From(embeddingResults[i].ItemId);
            if (!triedArticleIds.Contains(articleId))
            {
                triedArticleIds.Add(articleId);
                var article = await this.GetArticle(articleId.Value);
                if (tokenCount + article.TokenCount.Value < maxTokenCount.Value)
                {
                    results.Add(new Entities.Document()
                    {
                        Id = article.Id.Value,
                        FullText = article.Content.Value,
                        Title = article.Title.Value,
                        ContentChunks = article.Chunks?.Select(c => c.ChunkText.Value) ?? []
                    });
                    tokenCount += article.TokenCount.Value;
                }
            }
            i++;
        }

        return results;
    }

    public async Task<Article> GetArticle(string id)
    {
        return await _contentReadRepo.GetArticle(id);
    }

    public async Task<bool> ArticleExists(string id)
    {
        return await _contentReadRepo.ArticleExists(id);
    }
}
