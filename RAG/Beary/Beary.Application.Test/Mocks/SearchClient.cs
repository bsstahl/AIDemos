using Beary.Entities;
using Beary.Interfaces;

namespace Beary.Application.Test.Mocks;

[ExcludeFromCodeCoverage]
internal class SearchClient : IFindRelevantDocuments
{
    private readonly Mock<IFindRelevantDocuments> _searchClient = new();

    public Task<IEnumerable<SearchResult>> GetMostRelevant(IEnumerable<float> queryVector, int numberOfNeighbors)
    {
        return _searchClient.Object.GetMostRelevant(queryVector, numberOfNeighbors);
    }
}
