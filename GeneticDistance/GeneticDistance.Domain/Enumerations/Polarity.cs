namespace GeneticDistance.Domain.Enumerations;

public enum Polarity
{
	Neutral,        // No inherent positive or negative charge
	Positive,       // Favorable, beneficial, desirable
	Negative,       // Unfavorable, harmful, undesirable

	IntenselyPositive, // Strongly positive (excellent, wonderful)
	IntenselyNegative, // Strongly negative (terrible, horrific)

	Mixed,          // Contains both positive and negative aspects
	Contextual      // Polarity depends entirely on usage or domain
}
