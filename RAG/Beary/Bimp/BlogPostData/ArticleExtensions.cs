using Azure;

namespace Bimp.BlogPostData;

internal static class ArticleExtensions
{
    internal static IEnumerable<string> GetChunkGroups(this Article article)
    {
        int chunkCount = article.Chunks.Count;
        int i = 0;
        var result = new List<string>();
        do
        {
            string chunkText;
            var chunk = article.Chunks.SingleOrDefault(c => c.ChunkIndex == i);

            if (chunk is null)
                Console.WriteLine($"Null chunk in article {article.Id} at index {i}");
            else
            {
                if (chunk.IsHeader)
                {
                    // Combine with the next chunk
                    var nextChunk = article.Chunks.Single(c => c.ChunkIndex == i + 1);
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
