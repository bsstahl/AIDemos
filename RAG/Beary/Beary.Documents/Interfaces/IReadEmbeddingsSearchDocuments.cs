using Beary.Entities;
using Beary.ValueTypes;

namespace Beary.Documents.Interfaces;

public interface IReadEmbeddingsSearchDocuments
{
    Task<long> GetDocumentCount();
    Task<IEnumerable<SearchResult>> GetNearestNeighbors(IEnumerable<float> queryVector, int numberOfNeighbors);
    Task<IEnumerable<SearchResult>> GetAllEmbeddings();
}