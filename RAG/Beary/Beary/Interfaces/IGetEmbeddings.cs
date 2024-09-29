using Beary.Entities;

namespace Beary.Interfaces;

public interface IGetEmbeddings
{
    Task<ContentChunk?> GetEmbedding(string inputText, string baseId);
    Task<ContentChunk?> GetEmbedding(string inputText, string baseId, bool sanitizeInputs);

    Task<IEnumerable<ContentChunk>> GetEmbeddings(IEnumerable<string> inputText, string baseId);
    Task<IEnumerable<ContentChunk>> GetEmbeddings(IEnumerable<string> inputText, string baseId, bool sanitizeInputs);
}
