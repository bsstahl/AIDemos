using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AxiomVectorRepository;

public class ReadRepo : IGetAxiomVectors
{
    private readonly QdrantClient _client;

    public ReadRepo(QdrantClient client)
    {
        _client = client;
    }

    public Task<Axiom?> GetAxiomVector(String id) => throw new NotImplementedException();

    public async Task<IEnumerable<Axiom>> GetAxiomVectors(uint maxCount = 1000)
    {
        var results = new List<Axiom>();

        PointId? offset = null;
        do
        {
            var queryResult = await _client.ScrollAsync(
                Constants.CollectionName,
                limit: maxCount,
                payloadSelector: true,
                vectorsSelector: true,
                offset: offset);

            results.AddRange(queryResult.Result.Select(r => r.AsAxiom()));
            offset = queryResult.NextPageOffset;
        } while (offset is not null);

        return results;
    }

    public async Task<Axiom?> GetNearestAxiom(Single[] vector)
    {
        var result = await _client.QueryAsync(Constants.CollectionName,
            vector,
            limit: 1,
            payloadSelector: true,
            vectorsSelector: true);

        return result.FirstOrDefault()?.AsAxiom();
    }
}
