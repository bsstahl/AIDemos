using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AddAxioms;

internal class WriteRepo : IWriteAxiomEmbeddings
{
    private const string _collectionName = "axioms_collection";
    private const ulong _vectorSize = 768;

    private readonly QdrantClient _client;

    public WriteRepo(QdrantClient client)
    {
        _client = client;
    }

    public async Task AddAxiomAsync(string axiomText, float[] embedding)
    {
        var vectorConfig = new VectorParams
        {
            Size = _vectorSize,
            Distance = Distance.Cosine
        };

        var collectionExists = await _client.CollectionExistsAsync(_collectionName);
        if (!collectionExists)
		    await _client.CreateCollectionAsync(_collectionName, vectorConfig);

        var point = new PointStruct
        {
            Id = Guid.NewGuid(),
            Vectors = embedding,
            Payload =
                {
                    ["text"] = axiomText
                }
        };

        await _client.UpsertAsync(_collectionName, new[] { point });
    }
}