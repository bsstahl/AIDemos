namespace GeneticDistance.Domain.Enumerations;

public enum Concreteness
{
	Concrete,           // Physical, perceptible, directly experienced (apple, chair)
	SemiConcrete,       // Partly physical or indirectly perceptible (shadow, footprint)
	Abstract,           // Non‑physical concepts (justice, freedom)

	HighlyConcrete,     // Strong sensory grounding (sandpaper, thunder)
	HighlyAbstract,     // Purely conceptual (ontology, causation)

	DomainSpecific,     // Concrete within a domain but abstract outside it (quark, algorithm)
	Metaphorical,       // Concrete form used for abstract meaning (foundation, bridge)
	ContextDependent    // Concreteness varies by usage (charge, field, model)
}
