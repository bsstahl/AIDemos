namespace GeneticDistance.Domain.Enumerations;

public enum Animacy
{
	Inanimate,          // Objects, substances, abstract concepts
	Animate,            // Living things in general (broad category)

	Human,              // Specifically human referents
	NonHumanAnimal,     // Animals other than humans

	Collective,         // Groups treated as a single entity (team, committee)
	Agentive,           // Capable of intentional action (AI, institutions, deities)
	NonAgentive,        // Animate but not intentional (plants, microbes)

	Personified,        // Inanimate entities treated as animate (the law, fate, ships)
	Unknown             // When animacy cannot be determined
}
