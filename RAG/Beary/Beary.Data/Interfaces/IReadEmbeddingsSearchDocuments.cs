using Beary.Data.Entities;
using Beary.ValueTypes;

namespace Beary.Data.Interfaces;

public interface IReadEmbeddingsSearchDocuments
{
    Task<long> GetDocumentCount();
    Task<IEnumerable<SearchResult>> GetNearestNeighbors(Vector queryVector, ResultCount numberOfNeighbors);
    Task<IEnumerable<SearchResult>> GetAllEmbeddings();
}