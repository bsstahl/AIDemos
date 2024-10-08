using Beary.Entities;

namespace Beary.Chat.Interfaces;

public interface IGetRelevantDocuments
{
    Task<IEnumerable<SearchResult>> GetMostRelevant(IEnumerable<float> queryVector, int numberOfNeighbors);
}
