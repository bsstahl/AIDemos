using Beary.Documents.Interfaces;
using Beary.Entities;
using Beary.Interfaces;
using Beary.ValueTypes;
using SharpToken;

namespace Beary.Documents;

public class Import
{
    private readonly IReadSourceDocuments _sourceRepo;
    private readonly IGetEmbeddings _embeddingsClient;
    private readonly IWriteContentSearchDocuments _contentRepo;
    private readonly IWriteEmbeddingsSearchDocuments _chunkRepo;
    private readonly GptEncoding _encodingModel;

    public Import(IReadSourceDocuments sourceRepo, IGetEmbeddings embeddingsClient, IWriteContentSearchDocuments contentRepo, IWriteEmbeddingsSearchDocuments chunkRepo)
    {
        _sourceRepo = sourceRepo;
        _embeddingsClient = embeddingsClient;
        _contentRepo = contentRepo;
        _chunkRepo = chunkRepo;

        // TODO: Make Configurable
        _encodingModel = SharpToken.GptEncoding.GetEncodingForModel("gpt-4");
    }

    public async Task Execute()
    {
        var blogPosts = await _sourceRepo.GetAllDocuments();

        int i = 0;
        foreach (var article in blogPosts)
        {
            i++;
            var articleText = article.FullText;

            var tokens = _encodingModel.Encode(articleText);
            var tokenCount = TokenCount.From(tokens.Count);

            var embeddings = new List<Beary.Entities.ContentChunk>();
            var embeddingResults = await _embeddingsClient
                .GetEmbeddings(article.ContentChunks, article.Id)
                .ConfigureAwait(false);
            embeddings.AddRange(embeddingResults);

            await this.SaveAsync(Identifier.From(article.Id),
                ArticleTitle.From(article.Title),
                ArticleContent.From(articleText),
                tokenCount, embeddings);
        }
    }

    //internal async Task SaveAsync(Identifier articleId, ArticleTitle title, ArticleContent fullText)
    //{
    //    // 1. Get the TokenCount for the full text of the article.
    //    // 2. Break up the article into appropriately sized chunks.
    //    // 3. Get the Embedding vector for each chunk.
    //    // 4. Call the next overload of SaveAsync to persist the data.
    //    throw new NotImplementedException();
    //}

    internal async Task SaveAsync(Identifier articleId, ArticleTitle title, ArticleContent fullText, TokenCount articleTokens, IEnumerable<ContentChunk> chunks)
    {
        // 1. Update the Content repository with the full text of the article.
        // 2. Update the Search repository with each chunk along
        //    with the article's metadata including Embedding vector.

        // TODO: Check for nulls where they should be disallowed (ie articleId, fullText, articleTokens, Chunk.Id, Chunk.ChunkText)
        await _contentRepo.SaveAsync(articleId, title, fullText, articleTokens);
        foreach (var chunk in chunks)
            await _chunkRepo.SaveAsync(chunk.Id, chunk.Index, chunk.ChunkText, articleId, chunk.Embedding);
    }
}
