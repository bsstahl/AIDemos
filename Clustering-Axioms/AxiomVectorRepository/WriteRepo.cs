using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AxiomVectorRepository;

internal class WriteRepo : IWriteAxiomEmbeddings
{
    private readonly QdrantClient _client;

    public WriteRepo(QdrantClient client)
    {
        _client = client;
    }

    public async Task AddAxiomAsync(string axiomText, float[] embedding)
    {
        var vectorConfig = new VectorParams
        {
            Size = Constants.VectorSize,
            Distance = Distance.Cosine
        };

        var collectionExists = await _client.CollectionExistsAsync(Constants.CollectionName);
        if (!collectionExists)
		    await _client.CreateCollectionAsync(Constants.CollectionName, vectorConfig);

        var point = new PointStruct
        {
            Id = Guid.NewGuid(),
            Vectors = embedding,
            Payload =
                {
                    ["text"] = axiomText
                }
        };

        await _client.UpsertAsync(Constants.CollectionName, new[] { point });
    }
}