namespace GeneticDistance.Domain.Enumerations;

public enum Idiomaticity
{
	Literal,            // Meaning is fully compositional and transparent
	SemiIdiomatic,      // Partially compositional; some metaphorical drift
	Idiomatic,          // Meaning cannot be derived from the parts
	FixedExpression,    // Set phrase with little or no internal variation
	Collocation,        // Words that commonly co-occur but remain literal
	Proverbial,         // Traditional sayings with figurative meaning
	Metaphorical,       // Figurative usage that is not a fixed idiom
	ContextDependent    // Meaning shifts significantly by context
}
