namespace Beary.Data;

public class WriteRepository
{
    // Orchestrates the 2 underlying repositories
    // Process steps:
    // 1. Update the Content repository with the full text of the article.
    // 2. Break up the article into appropriately sized chunks.
    // 3. Update the Search repository with each chunk along
    //    with the article's metadata including Embedding vector.

}
