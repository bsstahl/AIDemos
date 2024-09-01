namespace Beary.Data.Articles;

public class ReadRepository
{
    // Orchestrates the 2 underlying repositories and contains the logic to determine
    // which articles to return based on semantic search results.
    // Process steps:
    // 1. Get the search results from the Azure AI Search repository.
    // 2. Determine the n most relevant, unique aritcles (sometimes the same article
    //    will have multiple search results).
    // 3. Get the articles from the Content repository.
}
