using Beary.Entities;

namespace Beary.Application.Interfaces;

public interface IReadEmbeddingsSearchDocuments
{
    Task<long> GetDocumentCount();
    Task<IEnumerable<SearchResult>> GetNearestNeighbors(IEnumerable<float> queryVector, int numberOfNeighbors);
    Task<IEnumerable<SearchResult>> GetAllEmbeddings();
}