using Beary.Data.Db;
using Beary.Entities;
using Beary.ValueTypes;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Security.Cryptography;
using Google.Protobuf.WellKnownTypes;

namespace Beary.Data.Qdrant;

internal sealed class QdrantDataStore : IBearyDataStore
{
    private const string ContentCollectionName = "beary-content";
    private const string EmbeddingsCollectionName = "beary-embeddings";

    private readonly QdrantClient _client;
    private readonly uint _vectorSize;

    private bool _contentCollectionReady;
    private bool _embeddingsCollectionReady;

    public QdrantDataStore(string host, int grpcPort, string? apiKey, uint vectorSize)
    {
        ArgumentException.ThrowIfNullOrEmpty(host, nameof(host));

        _vectorSize = vectorSize;
        _client = string.IsNullOrWhiteSpace(apiKey)
            ? new QdrantClient(host, grpcPort)
            : new QdrantClient(host, grpcPort, false, apiKey);
    }

    public async Task SaveArticleAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(tokenCount);

        await EnsureContentCollectionAsync().ConfigureAwait(false);

        var point = new PointStruct
        {
            Id = CreatePointId(id.Value),
            Vectors = new float[] { 0.0f },
            Payload =
            {
                ["articleId"] = id.Value,
                ["title"] = title?.Value ?? string.Empty,
                ["content"] = content.Value,
                ["tokenCount"] = tokenCount.Value
            }
        };

        await _client.UpsertAsync(ContentCollectionName, new[] { point }).ConfigureAwait(false);
    }

    public async Task SaveEmbeddingAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(elementIndex);
        ArgumentNullException.ThrowIfNull(contentChunk);
        ArgumentNullException.ThrowIfNull(fullArticleId);

        await EnsureEmbeddingsCollectionAsync().ConfigureAwait(false);

        var vector = embedding?.Value?.ToArray() ?? new float[_vectorSize];
        if (vector.Length != _vectorSize)
            throw new ArgumentException($"Embedding vector size mismatch. Expected {_vectorSize}, got {vector.Length}.");

        var point = new PointStruct
        {
            Id = CreatePointId(id.Value),
            Vectors = vector,
            Payload =
            {
                ["id"] = id.Value,
                ["elementIndex"] = elementIndex.Value,
                ["content"] = contentChunk.Value,
                ["articleId"] = fullArticleId.Value
            }
        };

        await _client.UpsertAsync(EmbeddingsCollectionName, new[] { point }).ConfigureAwait(false);
    }

    public async Task<Article> GetArticleAsync(string articleId)
    {
        ArgumentException.ThrowIfNullOrEmpty(articleId);

        if (!await _client.CollectionExistsAsync(ContentCollectionName).ConfigureAwait(false))
            throw new InvalidOperationException($"Article '{articleId}' was not found.");

        var pointId = CreatePointId(articleId);
        var articles = await _client.RetrieveAsync(ContentCollectionName, new[] { pointId }, withPayload: true).ConfigureAwait(false);
        var point = articles.FirstOrDefault()
            ?? throw new InvalidOperationException($"Article '{articleId}' was not found.");

        return new Article(
            ReadPayloadString(point.Payload, "articleId"),
            ReadPayloadString(point.Payload, "title"),
            ReadPayloadString(point.Payload, "content"),
            ReadPayloadInt(point.Payload, "tokenCount"));
    }

    public async Task<bool> ArticleExistsAsync(string articleId)
    {
        ArgumentException.ThrowIfNullOrEmpty(articleId);

        if (!await _client.CollectionExistsAsync(ContentCollectionName).ConfigureAwait(false))
            return false;

        var pointId = CreatePointId(articleId);
        var articles = await _client.RetrieveAsync(ContentCollectionName, new[] { pointId }, withPayload: false).ConfigureAwait(false);
        return articles.Any();
    }

    public async Task<long> GetEmbeddingDocumentCountAsync()
    {
        if (!await _client.CollectionExistsAsync(EmbeddingsCollectionName).ConfigureAwait(false))
            return 0;

        var info = await _client.GetCollectionInfoAsync(EmbeddingsCollectionName).ConfigureAwait(false);
        return info.PointsCount;
    }

    public async Task<IEnumerable<SearchResult>> GetNearestNeighborsAsync(IEnumerable<float> queryVector, int neighborCount)
    {
        ArgumentNullException.ThrowIfNull(queryVector);

        var searchVector = queryVector.ToArray();
        if (searchVector.Length != _vectorSize)
            throw new ArgumentException($"Query vector size mismatch. Expected {_vectorSize}, got {searchVector.Length}.");

        if (!await _client.CollectionExistsAsync(EmbeddingsCollectionName).ConfigureAwait(false))
            return [];

        var points = await _client.QueryAsync(
            EmbeddingsCollectionName,
            searchVector,
            limit: (ulong)neighborCount,
            payloadSelector: true,
            vectorsSelector: true).ConfigureAwait(false);

        return points.Select(p => new SearchResult
        {
            Id = ReadPayloadString(p.Payload, "id"),
            ElementIndex = ReadPayloadInt(p.Payload, "elementIndex"),
            Content = ReadPayloadString(p.Payload, "content"),
            ItemId = ReadPayloadString(p.Payload, "articleId"),
            Score = p.Score,
            Embedding = p.Vectors?.Vector?.Data?.ToArray() ?? []
        }).ToList();
    }

    public async Task<IEnumerable<SearchResult>> GetAllEmbeddingsAsync()
    {
        if (!await _client.CollectionExistsAsync(EmbeddingsCollectionName).ConfigureAwait(false))
            return [];

        var results = new List<SearchResult>();
        PointId? offset = null;
        do
        {
            var page = await _client.ScrollAsync(
                EmbeddingsCollectionName,
                limit: 1000,
                payloadSelector: true,
                vectorsSelector: true,
                offset: offset).ConfigureAwait(false);

            results.AddRange(page.Result.Select(r => new SearchResult
            {
                Id = ReadPayloadString(r.Payload, "id"),
                ElementIndex = ReadPayloadInt(r.Payload, "elementIndex"),
                Content = ReadPayloadString(r.Payload, "content"),
                ItemId = ReadPayloadString(r.Payload, "articleId"),
                Score = 0.0,
                Embedding = r.Vectors?.Vector?.Data?.ToArray() ?? []
            }));

            offset = page.NextPageOffset;
        }
        while (offset is not null);

        return results;
    }

    private async Task EnsureEmbeddingsCollectionAsync()
    {
        if (_embeddingsCollectionReady)
            return;

        if (!await _client.CollectionExistsAsync(EmbeddingsCollectionName).ConfigureAwait(false))
        {
            await _client.CreateCollectionAsync(EmbeddingsCollectionName, new VectorParams
            {
                Size = _vectorSize,
                Distance = Distance.Cosine
            }).ConfigureAwait(false);
        }

        _embeddingsCollectionReady = true;
    }

    private async Task EnsureContentCollectionAsync()
    {
        if (_contentCollectionReady)
            return;

        if (!await _client.CollectionExistsAsync(ContentCollectionName).ConfigureAwait(false))
        {
            await _client.CreateCollectionAsync(ContentCollectionName, new VectorParams
            {
                Size = 1,
                Distance = Distance.Cosine
            }).ConfigureAwait(false);
        }

        _contentCollectionReady = true;
    }

    private static PointId CreatePointId(string value)
    {
        if (Guid.TryParse(value, out var guid))
            return new PointId { Uuid = guid.ToString() };

        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value));
        var num = BitConverter.ToUInt64(bytes, 0);
        return new PointId { Num = num };
    }

    private static string ReadPayloadString(MapField<string, Value> payload, string key)
    {
        if (!payload.TryGetValue(key, out var value))
            return string.Empty;

        return value.StringValue;
    }

    private static int ReadPayloadInt(MapField<string, Value> payload, string key)
    {
        if (!payload.TryGetValue(key, out var value))
            return 0;

        return Convert.ToInt32(value.IntegerValue);
    }
}
