using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace BearyMC.Tools;

public class ArticlesListTool
{
    [McpServerTool, Description("Returns a list of relevant articles from my blog, Cognitive Inheritance.")]
    public async static Task<string> GetArticleList(ILogger<ArticlesListTool> logger, 
        Beary.Documents.Search searchClient, [Description("The phrase to perform a vector search on")]string searchQuery)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(searchClient, nameof(searchClient));

        logger.LogInformation("Received request to get articles for query: {SearchQuery}", searchQuery);

        var result = await searchClient.GetRelevantArticles(searchQuery, 4096);

        if (result is null || !result.Any())
        {
            logger.LogWarning("No articles found for query: {SearchQuery}", searchQuery);
            return "No articles found for the given query.";
        }
        else
        {
            logger.LogInformation("Found {ArticleCount} articles for query: {SearchQuery}", result.Count(), searchQuery);
            logger.LogDebug("Articles: {Articles}", string.Join(", ", result.Select(a => a.Title)));
        }

        return string.Join("\r\n", result.ToList());
    }
}
