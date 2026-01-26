using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticDistance.Domain;

public static class EnumerationExtensions
{
	public static TEnum GetRandom<TEnum>(this TEnum[] values) where TEnum : struct, Enum
	{
		var random = new Random();
		int index = random.Next(values.Length);
		return values[index];
	}
}
