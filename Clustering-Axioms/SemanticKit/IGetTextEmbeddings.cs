
namespace SemanticKit;

public interface IGetTextEmbeddings
{
    Task<Single[]> GetEmbeddingAsync(String textToEmbed);
    Task<IEnumerable<Single[]>> GetEmbeddingsAsync(IEnumerable<String> dataToEmbed);
}