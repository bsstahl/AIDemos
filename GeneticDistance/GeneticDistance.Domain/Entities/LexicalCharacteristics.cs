using System.Text.Json;
using System.Text.Json.Serialization;
using GeneticDistance.Domain.Enumerations;

namespace GeneticDistance.Domain.Entities;

[JsonConverter(typeof(LexicalCharacteristicsJsonConverter))]
public class LexicalCharacteristics
{
	public PartOfSpeech PartOfSpeech { get; set; }
	public Register Register { get; set; }
	public ScientificDiscipline ScientificDiscipline { get; set; }
	public Morphology Morphology { get; set; }
	public Animacy Animacy { get; set; }
	public Polarity Polarity { get; set; }
	public Idiomaticity Idiomaticity { get; set; }
	public Concreteness Concreteness { get; set; }

	public LexicalCharacteristics(string partOfSpeech, string register, string scientificDiscipline,
		string morphology, string animacy, string polarity, string idiomaticity,
		string concreteness)
		: this(Enum.Parse<PartOfSpeech>(partOfSpeech), Enum.Parse<Register>(register),
			Enum.Parse<ScientificDiscipline>(scientificDiscipline),
			Enum.Parse<Morphology>(morphology), Enum.Parse<Animacy>(animacy),
			Enum.Parse<Polarity>(polarity), Enum.Parse<Idiomaticity>(idiomaticity),
			Enum.Parse<Concreteness>(concreteness))
		{ }

	[JsonConstructor]
	public LexicalCharacteristics(PartOfSpeech partOfSpeech, Register register,
		ScientificDiscipline scientificDiscipline, Morphology morphology,
		Animacy animacy, Polarity polarity, Idiomaticity idiomaticity,
		Concreteness concreteness)
	{
		this.PartOfSpeech = partOfSpeech;
		this.Register = register;
		this.ScientificDiscipline = scientificDiscipline;
		this.Morphology = morphology;
		this.Animacy = animacy;
		this.Polarity = polarity;
		this.Idiomaticity = idiomaticity;
		this.Concreteness = concreteness;
	}

	public static LexicalCharacteristics GetRandom()
		=> new LexicalCharacteristics(
			Enum.GetValues<PartOfSpeech>().GetRandom().ToString(),
			Enum.GetValues<Animacy>().GetRandom().ToString(),
			Enum.GetValues<Concreteness>().GetRandom().ToString(),
			Enum.GetValues<Idiomaticity>().GetRandom().ToString(),
			Enum.GetValues<Morphology>().GetRandom().ToString(),
			Enum.GetValues<Polarity>().GetRandom().ToString(),
			Enum.GetValues<Register>().GetRandom().ToString(),
			Enum.GetValues<ScientificDiscipline>().GetRandom().ToString());
}

internal sealed class LexicalCharacteristicsJsonConverter : JsonConverter<LexicalCharacteristics>
{
	public override LexicalCharacteristics? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
			return null;

		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException("Expected start of object for LexicalCharacteristics.");

		PartOfSpeech? partOfSpeech = null;
		Register? register = null;
		ScientificDiscipline? scientificDiscipline = null;
		Morphology? morphology = null;
		Animacy? animacy = null;
		Polarity? polarity = null;
		Idiomaticity? idiomaticity = null;
		Concreteness? concreteness = null;

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;

			if (reader.TokenType != JsonTokenType.PropertyName)
				continue;

			var propName = reader.GetString();
			reader.Read();

			if (string.Equals(propName, nameof(LexicalCharacteristics.PartOfSpeech), StringComparison.OrdinalIgnoreCase))
				partOfSpeech = ReadEnumValue<PartOfSpeech>(ref reader);
			else if (string.Equals(propName, nameof(LexicalCharacteristics.Register), StringComparison.OrdinalIgnoreCase))
				register = ReadEnumValue<Register>(ref reader);
			else if (string.Equals(propName, nameof(LexicalCharacteristics.ScientificDiscipline), StringComparison.OrdinalIgnoreCase))
				scientificDiscipline = ReadEnumValue<ScientificDiscipline>(ref reader);
			else if (string.Equals(propName, nameof(LexicalCharacteristics.Morphology), StringComparison.OrdinalIgnoreCase))
				morphology = ReadEnumValue<Morphology>(ref reader);
			else if (string.Equals(propName, nameof(LexicalCharacteristics.Animacy), StringComparison.OrdinalIgnoreCase))
				animacy = ReadEnumValue<Animacy>(ref reader);
			else if (string.Equals(propName, nameof(LexicalCharacteristics.Polarity), StringComparison.OrdinalIgnoreCase))
				polarity = ReadEnumValue<Polarity>(ref reader);
			else if (string.Equals(propName, nameof(LexicalCharacteristics.Idiomaticity), StringComparison.OrdinalIgnoreCase))
				idiomaticity = ReadEnumValue<Idiomaticity>(ref reader);
			else if (string.Equals(propName, nameof(LexicalCharacteristics.Concreteness), StringComparison.OrdinalIgnoreCase))
				concreteness = ReadEnumValue<Concreteness>(ref reader);
			else
				reader.Skip();
		}

		// Ensure required fields are present
		return partOfSpeech is null || register is null || scientificDiscipline is null || morphology is null ||
			animacy is null || polarity is null || idiomaticity is null || concreteness is null
			? throw new JsonException("Missing one or more LexicalCharacteristics properties.")
			: new LexicalCharacteristics(partOfSpeech.Value, register.Value, scientificDiscipline.Value, morphology.Value,
				animacy.Value, polarity.Value, idiomaticity.Value, concreteness.Value);
	}

	public override void Write(Utf8JsonWriter writer, LexicalCharacteristics value, JsonSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteStartObject();
		writer.WriteString(nameof(LexicalCharacteristics.PartOfSpeech), value.PartOfSpeech.ToString());
		writer.WriteString(nameof(LexicalCharacteristics.Register), value.Register.ToString());
		writer.WriteString(nameof(LexicalCharacteristics.ScientificDiscipline), value.ScientificDiscipline.ToString());
		writer.WriteString(nameof(LexicalCharacteristics.Morphology), value.Morphology.ToString());
		writer.WriteString(nameof(LexicalCharacteristics.Animacy), value.Animacy.ToString());
		writer.WriteString(nameof(LexicalCharacteristics.Polarity), value.Polarity.ToString());
		writer.WriteString(nameof(LexicalCharacteristics.Idiomaticity), value.Idiomaticity.ToString());
		writer.WriteString(nameof(LexicalCharacteristics.Concreteness), value.Concreteness.ToString());
		writer.WriteEndObject();
	}

	private static T ReadEnumValue<T>(ref Utf8JsonReader reader) where T : struct, Enum
	{
		if (reader.TokenType == JsonTokenType.String)
		{
			var s = reader.GetString()!;
			return Enum.TryParse<T>(s, true, out var result)
				? result
				: throw new JsonException($"Invalid enum value '{s}' for {typeof(T).Name}.");
		}

		return reader.TokenType == JsonTokenType.Number
			? reader.TryGetInt32(out int intVal) && Enum.IsDefined(typeof(T), intVal)
				? (T)Enum.ToObject(typeof(T), intVal)
				: throw new JsonException($"Invalid numeric enum value for {typeof(T).Name}.")
			: throw new JsonException($"Unexpected token parsing enum {typeof(T).Name}: {reader.TokenType}.");
	}
}
