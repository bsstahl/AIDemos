namespace GeneticDistance.Domain.Enumerations;

public enum Morphology
{
	Root,               // Base form with no affixes
	Stem,               // Root plus derivational material

	Inflected,          // Any inflectional change
	Derivative,         // Any derivational change

	Prefix,             // Has a prefix
	Suffix,             // Has a suffix
	Infix,              // Rare in English, but included for completeness
	Circumfix,          // Also rare, but morphologically valid

	Compound,           // Two or more roots combined
	Reduplication,      // Partial or full repetition (rare in English)
	Clitic,             // Contracted or attached form (’s, ’ll, etc.)

	RegularInflection,  // Standard morphological pattern
	IrregularInflection // Non‑standard pattern (go → went)
}
