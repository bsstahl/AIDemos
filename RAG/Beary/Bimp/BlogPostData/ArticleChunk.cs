namespace Bimp.BlogPostData;

public class ArticleChunk
{
    public int ChunkIndex { get; set; }

    public bool IsHeader { get; set; }

    public string ChunkText { get; set; } = string.Empty;


    public string? DisambiguatedText { get; set; } = null;

    public IEnumerable<float>? ChunkEmbedding { get; set; } = null;


    public ArticleChunk()
    { }

    public ArticleChunk(int chunkIndex, string chunkText, bool isHeader)
    {
        this.ChunkIndex = chunkIndex;
        this.ChunkText = chunkText;
        this.IsHeader = isHeader;
    }

    public ArticleChunk(int chunkIndex, string chunkText, bool isHeader, string? disambiguatedText, IEnumerable<float>? chunkEmbedding)
    {
        this.ChunkIndex = chunkIndex;
        this.ChunkText = chunkText;
        this.IsHeader = isHeader;
        this.DisambiguatedText = disambiguatedText;
        this.ChunkEmbedding = chunkEmbedding;
    }
}
