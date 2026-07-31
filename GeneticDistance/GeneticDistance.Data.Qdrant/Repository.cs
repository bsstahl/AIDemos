using GeneticDistance.Domain.Entities;
using GeneticDistance.Domain.Interfaces;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using DomainVector = GeneticDistance.Domain.ValueTypes.Vector;
using DomainExpression = GeneticDistance.Domain.Entities.Expression;
using GrpcVector = Qdrant.Client.Grpc.Vector;

namespace GeneticDistance.Data.Qdrant;

public class Repository : IEmbeddingRepository
{
	private const string CollectionName = "geneticdistance-embeddings";
	private const string OriginalTextPayloadKey = "originalText";
	private const string NormalizedTextPayloadKey = "normalizedText";
	private const string CharPartOfSpeechKey = "char_partOfSpeech";
	private const string CharRegisterKey = "char_register";
	private const string CharScientificDisciplineKey = "char_scientificDiscipline";
	private const string CharMorphologyKey = "char_morphology";
	private const string CharAnimacyKey = "char_animacy";
	private const string CharPolarityKey = "char_polarity";
	private const string CharIdiomaticityKey = "char_idiomaticity";
	private const string CharConcretnessKey = "char_concreteness";
	private const int VectorDimensions = 768;

	private readonly QdrantClient _client;
	private readonly SemaphoreSlim _collectionLock = new(1, 1);
	private bool _collectionInitialized;

	public Repository(QdrantClient client)
	{
		_client = client;
	}

	public async Task<IReadOnlyList<DomainExpression>> GetAllAsync(Int32 batchSize = 500)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize, nameof(batchSize));
		await EnsureCollectionAsync();

		var expressions = new List<DomainExpression>();
		PointId? offset = null;

		while (true)
		{
			var page = await _client.ScrollAsync(
				collectionName: CollectionName,
				filter: null,
				limit: (uint)batchSize,
				offset: offset,
				payloadSelector: new WithPayloadSelector { Enable = true },
				vectorsSelector: new WithVectorsSelector { Enable = true });

			if (page.Result.Count == 0)
				break;

			expressions.AddRange(page.Result.Select(ToExpression));

			if (page.NextPageOffset is null || page.NextPageOffset.PointIdOptionsCase == PointId.PointIdOptionsOneofCase.None)
				break;

			offset = page.NextPageOffset;
		}

		return expressions;
	}

	public async Task<DomainExpression?> GetByIdAsync(String id)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
		await EnsureCollectionAsync();

		var records = await _client.RetrieveAsync(
			collectionName: CollectionName,
			id: new PointId { Uuid = id },
			withPayload: true,
			withVectors: true);

		var point = records.FirstOrDefault();
		return point is null ? null : ToExpression(point);
	}

	public async Task<DomainExpression?> GetByTextAsync(String normalizedText)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(normalizedText, nameof(normalizedText));
		await EnsureCollectionAsync();

		var filter = new Filter();
		filter.Must.Add(new Condition
		{
			Field = new FieldCondition
			{
				Key = NormalizedTextPayloadKey,
				Match = new Match { Keyword = normalizedText.NormalizeText() }
			}
		});

		var result = await _client.ScrollAsync(
			collectionName: CollectionName,
			filter: filter,
			limit: 1,
			offset: null,
			payloadSelector: new WithPayloadSelector { Enable = true },
			vectorsSelector: new WithVectorsSelector { Enable = true });

		var point = result.Result.FirstOrDefault();
		return point is null ? null : ToExpression(point);
	}

	public async Task<String> GetOrCreateAsync(String id, String originalText, Single[] vector, LexicalCharacteristics? characteristics = null)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
		ArgumentException.ThrowIfNullOrWhiteSpace(originalText, nameof(originalText));
		ArgumentNullException.ThrowIfNull(vector, nameof(vector));

		if (vector.Length != VectorDimensions)
			throw new ArgumentException($"Expected vector with {VectorDimensions} dimensions.", nameof(vector));

		await EnsureCollectionAsync();

		var normalizedText = originalText.NormalizeText();
		var existing = await GetByTextAsync(normalizedText);
		if (existing?.Id is not null)
			return existing.Id;

		var point = new PointStruct
		{
			Id = new PointId { Uuid = id },
			Vectors = new Vectors
			{
				Vector = new GrpcVector
				{
					Dense = new DenseVector()
				}
			}
		};

		point.Vectors.Vector.Dense.Data.Add(vector);
		point.Payload.Add(OriginalTextPayloadKey, new Value { StringValue = originalText });
		point.Payload.Add(NormalizedTextPayloadKey, new Value { StringValue = normalizedText });

		if (characteristics is not null)
		{
			point.Payload.Add(CharPartOfSpeechKey, new Value { StringValue = characteristics.PartOfSpeech.ToString() });
			point.Payload.Add(CharRegisterKey, new Value { StringValue = characteristics.Register.ToString() });
			point.Payload.Add(CharScientificDisciplineKey, new Value { StringValue = characteristics.ScientificDiscipline.ToString() });
			point.Payload.Add(CharMorphologyKey, new Value { StringValue = characteristics.Morphology.ToString() });
			point.Payload.Add(CharAnimacyKey, new Value { StringValue = characteristics.Animacy.ToString() });
			point.Payload.Add(CharPolarityKey, new Value { StringValue = characteristics.Polarity.ToString() });
			point.Payload.Add(CharIdiomaticityKey, new Value { StringValue = characteristics.Idiomaticity.ToString() });
			point.Payload.Add(CharConcretnessKey, new Value { StringValue = characteristics.Concreteness.ToString() });
		}

		await _client.UpsertAsync(
			collectionName: CollectionName,
			points: new[] { point },
			wait: true,
			ordering: null,
			shardKeySelector: null);

		return id;
	}

	private async Task EnsureCollectionAsync()
	{
		if (_collectionInitialized)
			return;

		await _collectionLock.WaitAsync();
		try
		{
			if (_collectionInitialized)
				return;

			var exists = await _client.CollectionExistsAsync(CollectionName);
			if (!exists)
			{
				await _client.CreateCollectionAsync(
					CollectionName,
					new VectorParams
					{
						Size = VectorDimensions,
						Distance = Distance.Cosine
					});
			}

			_collectionInitialized = true;
		}
		finally
		{
			_collectionLock.Release();
		}
	}

	private static DomainExpression ToExpression(RetrievedPoint point)
	{
		var text = GetRequiredStringPayload(point, OriginalTextPayloadKey);
		var id = ResolvePointId(point.Id);
		var vector = ReadVector(point.Vectors);
		var characteristics = TryReadCharacteristics(point);

		return new DomainExpression(text)
		{
			Id = id,
			Vector = DomainVector.From(vector),
			Characteristics = characteristics
		};
	}

	private static LexicalCharacteristics? TryReadCharacteristics(RetrievedPoint point)
	{
		if (!TryGetStringPayload(point, CharPartOfSpeechKey, out var partOfSpeech) ||
			!TryGetStringPayload(point, CharRegisterKey, out var register) ||
			!TryGetStringPayload(point, CharScientificDisciplineKey, out var scientificDiscipline) ||
			!TryGetStringPayload(point, CharMorphologyKey, out var morphology) ||
			!TryGetStringPayload(point, CharAnimacyKey, out var animacy) ||
			!TryGetStringPayload(point, CharPolarityKey, out var polarity) ||
			!TryGetStringPayload(point, CharIdiomaticityKey, out var idiomaticity) ||
			!TryGetStringPayload(point, CharConcretnessKey, out var concreteness))
			return null;

		try
		{
			return new LexicalCharacteristics(partOfSpeech!, register!, scientificDiscipline!,
				morphology!, animacy!, polarity!, idiomaticity!, concreteness!);
		}
		catch
		{
			return null;
		}
	}

	private static string ResolvePointId(PointId? pointId)
	{
		if (pointId is null)
			throw new InvalidOperationException("Point is missing an id.");

		return pointId.PointIdOptionsCase switch
		{
			PointId.PointIdOptionsOneofCase.Uuid => pointId.Uuid,
			PointId.PointIdOptionsOneofCase.Num => pointId.Num.ToString(),
			_ => throw new InvalidOperationException("Point id has no value.")
		};
	}

	private static string GetRequiredStringPayload(RetrievedPoint point, string payloadKey)
	{
		if (!point.Payload.TryGetValue(payloadKey, out var payloadValue) || !payloadValue.HasStringValue)
			throw new InvalidOperationException($"Point payload is missing required string key '{payloadKey}'.");

		return payloadValue.StringValue;
	}

	private static bool TryGetStringPayload(RetrievedPoint point, string payloadKey, out string? value)
	{
		if (point.Payload.TryGetValue(payloadKey, out var payloadValue) && payloadValue.HasStringValue)
		{
			value = payloadValue.StringValue;
			return true;
		}
		value = null;
		return false;
	}

	private static float[] ReadVector(VectorsOutput? vectors)
	{
		if (vectors?.Vector is null)
			throw new InvalidOperationException("Point is missing vector data.");

		if (vectors.Vector.Dense is not null && vectors.Vector.Dense.Data.Count > 0)
			return vectors.Vector.Dense.Data.ToArray();

		throw new InvalidOperationException("Point has empty vector data.");
	}
}
