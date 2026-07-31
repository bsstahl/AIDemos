using GeneticDistance.Domain.Entities;
using Microsoft.Extensions.AI;

namespace GeneticDistance.Execution.Extensions;

public static class OllamaClientExtensions
{
    public static ChatOptions SlmChatOptions 
        => new ChatOptions
        {
            ModelId = "llama3.2:1b",
            Temperature = 0.7f,
            TopP = 0.95f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f
        };

    public static ChatOptions ReasoningChatOptions 
        => new ChatOptions
        {
            ModelId = "llama3.2:1b",
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
        var doNotUseText = string.Join(", ", doNotUse);

		var chatMessages = new List<ChatMessage>
        {
            { new ChatMessage(ChatRole.System, "You are a simulation of a great linguist. You classify words and short phrases in lexical categories. Return exactly one candidate in lower-case text and no extra punctuation, explanation, labels, or code blocks.") },
            { new ChatMessage(ChatRole.User, $"Give me a word or phrase with the following characteristics:  - Primary part of speech is {target.PartOfSpeech} - Register is {target.Register} - Morphology is {target.Morphology} - Animacy is {target.Animacy} - Scientific Discipline is {target.ScientificDiscipline} - Polarity is {target.Polarity} - Idiomaticity {target.Idiomaticity} - Concreteness is {target.Concreteness}  and is not in the following list: {doNotUseText}.  Be sure to only respond with the selected word or phrase, no ceremony.") }
		};

        var response = await chatClient.GetResponseAsync(
            chatMessages,
            ReasoningChatOptions);

        return response.Text.Trim();
    }
}
