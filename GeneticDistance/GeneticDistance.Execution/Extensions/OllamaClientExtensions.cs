using GeneticDistance.Domain.Entities;
using Microsoft.Extensions.AI;

namespace GeneticDistance.Execution.Extensions;

public static class OllamaClientExtensions
{
    private const int MaxRejectedCandidateExamples = 12;

    public static ChatOptions SlmChatOptions 
        => new ChatOptions
        {
            ModelId = "qwen2.5:3b",
            Temperature = 0.7f,
            TopP = 0.95f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f
        };

    public static ChatOptions ReasoningChatOptions 
        => new ChatOptions
        {
            ModelId = "qwen2.5:3b",
            Temperature = 0.3f,
            TopP = 0.9f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f
        };

	public static EmbeddingGenerationOptions OllamaEmbeddingOptions 
        => new EmbeddingGenerationOptions
        {
            ModelId = "nomic-embed-text"
        };

    public async static Task<float[]> GetEmbeddingAsync(this IEmbeddingGenerator<string, Embedding<float>> embeddingClient, string value) 
        => (await embeddingClient.GenerateVectorAsync(value, OllamaEmbeddingOptions)).ToArray();

    public async static Task<string> GetCandidateAsync(this IChatClient chatClient, LexicalCharacteristics target, IEnumerable<string> doNotUse)
    {
        var rejectedCandidates = doNotUse
            .Where(candidate => !string.IsNullOrWhiteSpace(candidate))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(MaxRejectedCandidateExamples)
            .ToList();
        var doNotUseText = rejectedCandidates.Count == 0
            ? "none"
            : string.Join(", ", rejectedCandidates);

		var chatMessages = new List<ChatMessage>
        {
            { new ChatMessage(ChatRole.System, "You are a simulation of a great linguist. You supply real, standard English words and short phrases. The candidate must be a genuine, naturally-occurring English word or idiomatic phrase that a native speaker would recognise — it must exist in a standard English dictionary or be in common everyday use. Do NOT invent hyphenated compounds, technical-sounding fabrications, or grammatically incomplete fragments. Return exactly one lower-case candidate of one to five words. Never return a full sentence, explanation, list, label, punctuation-only response, or code block.") },
            { new ChatMessage(ChatRole.User, $"Give me a real, standard English word or short phrase with the following characteristics: - Primary part of speech is {target.PartOfSpeech} - Register is {target.Register} - Morphology is {target.Morphology} - Animacy is {target.Animacy} - Scientific Discipline is {target.ScientificDiscipline} - Polarity is {target.Polarity} - Idiomaticity is {target.Idiomaticity} - Concreteness is {target.Concreteness} The candidate must be a genuine, naturally-occurring English word or idiomatic phrase — not an invented compound, not a fragment, not a technical fabrication. Avoid every prior invalid candidate in this list: {doNotUseText}. Pick something substantially different from those prior invalid candidates. Respond with only the candidate text.") }
		};

        var response = await chatClient.GetResponseAsync(
            chatMessages,
            ReasoningChatOptions);

        return response.Text.Trim();
    }
}
