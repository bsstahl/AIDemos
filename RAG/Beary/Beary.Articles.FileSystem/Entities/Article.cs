namespace Beary.Articles.FileSystem.Entities;

internal class Article
{
    public Article(string articleId, string articleTitle, string articleDescription, IEnumerable<float>? descriptionEmbedding, bool isPublished, bool includeAlways, IEnumerable<string> articleCategories, IEnumerable<string> articleTags)
    {
        this.Id = articleId;
        this.Title = articleTitle;
        this.Description = articleDescription;
        this.DescriptionEmbedding = descriptionEmbedding;
        this.IsPublished = isPublished;
        this.IncludeAlways = includeAlways;
        this.Categories = articleCategories;
        this.Tags = articleTags;
    }

    public string Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public IEnumerable<float>? DescriptionEmbedding { get; set; }

    public bool IsPublished { get; set; } = false;
    public bool IncludeAlways { get; set; } = false;

    public IEnumerable<string> Categories { get; set; }
    public IEnumerable<string> Tags { get; set; }

    private List<ArticleChunk> _contentChunks = new List<ArticleChunk>();
    public List<ArticleChunk> Chunks
    {
        get => _contentChunks;
        protected set => _contentChunks = value;
    }

    public string GetFullArticleText()
        => string.Join("\r\n", _contentChunks
            .OrderBy(c => c.ChunkIndex)
            .Select(c => c.ChunkText));

    public void AddChunk(int paragraphIndex, string paragraphText, bool isHeader)
    {
        _contentChunks.Add(new ArticleChunk() { ChunkIndex = paragraphIndex, ChunkText = paragraphText, IsHeader = isHeader });
    }

    public void AddChunks(IEnumerable<ArticleChunk> chunks)
    {
        _contentChunks.AddRange(chunks);
    }

    internal IEnumerable<string> GetChunkGroups()
    {
        int chunkCount = this.Chunks.Count;
        int i = 0;
        var result = new List<string>();
        do
        {
            string chunkText;
            var chunk = this.Chunks.SingleOrDefault(c => c.ChunkIndex == i);

            if (chunk is null)
                Console.WriteLine($"Null chunk in article {this.Id} at index {i}");
            else
            {
                if (chunk.IsHeader)
                {
                    // Combine with the next chunk
                    var nextChunk = this.Chunks.Single(c => c.ChunkIndex == i + 1);
                    chunkText = $"{chunk.ChunkText}\r\n\r\n{nextChunk.ChunkText}";
                    i++; // Advance a 2nd time
                }
                else
                {
                    chunkText = chunk.ChunkText;
                }
                result.Add(chunkText);
            }
            i++;
        }
        while (i < chunkCount);

        return result;
    }
}
