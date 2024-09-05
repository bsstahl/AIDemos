using Beary.Data.Interfaces;
using Beary.Entities;
using Beary.Interfaces;
using Beary.ValueTypes;

namespace Beary.Data;

public class ReadRepository : IReadContent
{
    const int defaultArticleCount = 10;

    private readonly IReadEmbeddingsSearchDocuments _embeddingsReadRepo;
    private readonly IReadContentSearchDocuments _contentReadRepo;

    public ReadRepository(IReadEmbeddingsSearchDocuments embeddingsReadRepo, IReadContentSearchDocuments contentReadRepo)
    {
        _embeddingsReadRepo = embeddingsReadRepo;
        _contentReadRepo = contentReadRepo;
    }

    // Orchestrates the 2 underlying repositories and contains the logic to determine
    // which articles to return based on semantic search results.
    // Process steps:
    // 1. Get the search results from the Azure AI Search repository.
    // 2. Determine the n most relevant, unique articles (sometimes the same article
    //    will have multiple search results).
    // 3. Get the articles from the Content repository.
    public async Task<IEnumerable<Article>> GetRelevantArticles(Vector embedding, TokenCount maxTokenCount)
    {
        var results = new List<Article>();

        var nearestNeighbors = await _embeddingsReadRepo
            .GetNearestNeighbors(embedding, ResultCount.From(defaultArticleCount));

        int i = 0;
        int tokenCount = 0;
        var triedArticleIds = new List<Identifier>();
        var embeddingResults = nearestNeighbors.ToArray();
        while (tokenCount < maxTokenCount.Value && i < nearestNeighbors.Count())
        {
            // TODO: Ignore the article if the results are below a minimum score
            var articleId = Identifier.From(embeddingResults[i].ArticleId);
            if (!triedArticleIds.Contains(articleId))
            {
                triedArticleIds.Add(articleId);
                var article = await this.GetArticle(articleId);
                if (tokenCount + article.TokenCount.Value < maxTokenCount.Value)
                {
                    results.Add(article);
                    tokenCount += article.TokenCount.Value;
                }
            }
            i++;
        }

        return results;
    }

    public async Task<Article> GetArticle(Identifier id)
    {
        return await _contentReadRepo.GetArticle(id);
    }


}
