using Beary.Entities;

namespace Beary.Interfaces;

public interface IFindRelevantDocuments
{
    Task<IEnumerable<SearchResult>> GetMostRelevant(IEnumerable<float> queryVector, int numberOfNeighbors);
}
