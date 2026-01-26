namespace GeneticDistance.Domain.Entities;

public sealed class CharacteristicMap<TEnum> where TEnum : struct, Enum
{
	private readonly IReadOnlyDictionary<TEnum, TEnum> _map;

	public CharacteristicMap(IDictionary<TEnum, TEnum> map)
	{
		var allValues = Enum.GetValues<TEnum>();

		// Ensure completeness
		if (map.Count != allValues.Length)
			throw new ArgumentException(
				$"Mapping must include every {typeof(TEnum).Name} value.");

		// Ensure no missing keys
		foreach (var value in allValues)
		{
			if (!map.ContainsKey(value))
				throw new ArgumentException(
					$"Missing mapping for {value}.");
		}

		_map = new Dictionary<TEnum, TEnum>(map);
	}

	public TEnum Map(TEnum source) => _map[source];

	public IReadOnlyDictionary<TEnum, TEnum> AsDictionary() => _map;

	public static CharacteristicMap<TEnum> GetRandom()
	{
		var allValues = Enum.GetValues<TEnum>();
		var map = new Dictionary<TEnum, TEnum>();
		foreach (var value in allValues)
			map[value] = allValues.GetRandom();
		return new CharacteristicMap<TEnum>(map);
	}
}