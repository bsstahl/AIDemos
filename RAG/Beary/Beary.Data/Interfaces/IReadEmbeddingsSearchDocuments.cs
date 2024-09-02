using Beary.Data.Entities;
using Beary.ValueTypes;

namespace Beary.Data.Interfaces;

public interface IReadEmbeddingsSearchDocuments
{
    Task<IEnumerable<SearchResult>> GetNearestNeighbors(Vector queryVector, ResultCount numberOfNeighbors);
}