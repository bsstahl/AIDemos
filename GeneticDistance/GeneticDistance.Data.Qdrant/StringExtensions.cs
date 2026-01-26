using System.Text;
using System.Text.RegularExpressions;

namespace GeneticDistance.Data.Qdrant;

internal static class StringExtensions
{
	internal static string NormalizeText(this string input)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(input, nameof(input));

		// 1. Trim leading/trailing whitespace
		string text = input.Trim();

		// 2. Normalize Unicode to a canonical form
		// FormKC handles compatibility characters (e.g., full-width forms)
		text = text.Normalize(NormalizationForm.FormKC);

		// 3. Convert to lowercase (culture-invariant)
		text = text.ToLowerInvariant();

		// 4. Collapse internal whitespace (spaces, tabs, newlines)
		text = Regex.Replace(text, @"\s+", " ");

		// 5. Remove zero-width and non-printing characters
		text = text.RemoveInvisibleCharacters();

		return text;
	}

	private static string RemoveInvisibleCharacters(this string text)
	{
		// Removes characters that visually disappear but break equality checks
		// Includes: zero-width space, zero-width joiner, non-breaking space, etc.
		var builder = new StringBuilder(text.Length);

		foreach (var ch in text)
		{
			if (!char.IsControl(ch) &&
				ch != '\u200B' && // zero-width space
				ch != '\u200C' && // zero-width non-joiner
				ch != '\u200D' && // zero-width joiner
				ch != '\uFEFF')   // zero-width no-break space (BOM)
			{
				builder.Append(ch);
			}
		}

		return builder.ToString();
	}

}
