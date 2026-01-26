using System.Text.Json;
using System.Text.Json.Serialization;
using ValueOf;

namespace GeneticDistance.Domain.ValueTypes;

[JsonConverter(typeof(VectorJsonConverter))]
public class Vector : ValueOf<float[], Vector>
{
	const int _dimensionCount = 768;

	protected override void Validate()
	{
		if (!this.Value.Length.Equals(_dimensionCount))
			throw new ArgumentException($"A {nameof(Vector)} must have {_dimensionCount} dimensions.");
	}

	public float CosineDistanceFrom(Vector target)
	{
		if (target is null)
			throw new ArgumentNullException("Cannot compare to a null Vector.");

		var a = this.Value;
		var b = target.Value;

		float dot = 0f;
		float normA = 0f;
		float normB = 0f;

		for (int i = 0; i < a.Length; i++)
		{
			dot += a[i] * b[i];
			normA += a[i] * a[i];
			normB += b[i] * b[i];
		}

		float denom = (float)(Math.Sqrt(normA) * Math.Sqrt(normB));
		if (denom == 0f)
			throw new InvalidOperationException("Cosine distance undefined for zero‑magnitude vectors.");

		float cosineSimilarity = dot / denom;
		return 1f - cosineSimilarity;
	}

	private sealed class VectorJsonConverter : JsonConverter<Vector>
	{
		public override Vector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Null)
				return null!;

			if (reader.TokenType != JsonTokenType.StartArray)
				throw new JsonException("Expected start of array for Vector.");

			var list = new List<float>();
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndArray)
					break;

				if (reader.TokenType != JsonTokenType.Number)
					throw new JsonException("Expected numeric values inside the Vector array.");

				list.Add(reader.GetSingle());
			}

			return list.Count != _dimensionCount
				? throw new JsonException($"A {nameof(Vector)} must have {_dimensionCount} dimensions.")
				: From(list.ToArray());
		}

		public override void Write(Utf8JsonWriter writer, Vector value, JsonSerializerOptions options)
		{
			if (value is null)
			{
				writer.WriteNullValue();
				return;
			}

			writer.WriteStartArray();
			foreach (var v in value.Value)
				writer.WriteNumberValue(v);
			writer.WriteEndArray();
		}
	}
}
