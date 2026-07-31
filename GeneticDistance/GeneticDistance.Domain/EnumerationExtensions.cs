using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticDistance.Domain;

public static class EnumerationExtensions
{
	public static TEnum GetRandom<TEnum>(this TEnum[] values) where TEnum : struct, Enum
	{
		int index = Random.Shared.Next(values.Length);
		return values[index];
	}
}
