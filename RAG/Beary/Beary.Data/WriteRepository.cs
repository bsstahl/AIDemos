using Beary.Data.Entities;
using Beary.Data.Interfaces;
using Beary.ValueTypes;

namespace Beary.Data;

/// Orchestrates the 2 underlying repositories
public class WriteRepository
{
    private readonly IWriteContentSearchDocuments _contentRepo;
    private readonly IWriteEmbeddingsSearchDocuments _chunkRepo;

    public WriteRepository(IWriteContentSearchDocuments contentRepo, IWriteEmbeddingsSearchDocuments chunkRepo)
    {
        _contentRepo = contentRepo;
        _chunkRepo = chunkRepo;
    }

    public async Task SaveAsync(Identifier articleId, ArticleContent fullText)
    {
        // 1. Get the TokenCount for the full text of the article.
        // 2. Break up the article into appropriately sized chunks.
        // 3. Get the Embedding vector for each chunk.
        // 4. Call the next overload of SaveAsync to persist the data.
        throw new NotImplementedException();
    }

    public async Task SaveAsync(Identifier articleId, ArticleContent fullText, TokenCount articleTokens, IEnumerable<ContentChunk> chunks)
    {
        // 1. Update the Content repository with the full text of the article.
        // 2. Update the Search repository with each chunk along
        //    with the article's metadata including Embedding vector.

        // TODO: Check for nulls where they should be disallowed (ie articleId, fullText, articleTokens, Chunk.Id, Chunk.ChunkText)
        await _contentRepo.SaveAsync(articleId, fullText, articleTokens);
        foreach (var chunk in chunks)
        {
            await _chunkRepo.SaveAsync(chunk.Id, chunk.ChunkText, articleId, chunk.Embedding);
        }
        throw new NotImplementedException();
    }
}
