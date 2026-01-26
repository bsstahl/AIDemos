using GeneticDistance.Domain.Entities;
using Microsoft.Extensions.AI;

namespace GeneticDistance.Api.Extensions;

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
            ModelId = "gpt-oss:20b",
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
        // TODO: Implement fully to get a candidate word/phrase

        var doNotUseText = string.Join(",", doNotUse);

		var chatMessages = new List<ChatMessage>
        {
            { new ChatMessage(ChatRole.System, "You are a simulation of a great linguist. You classify words/phrases within the following categories:  - Part of Speech (noun, verb, etc) - Register (formal, slang, technical, etc) - Scientific Discipline (biology, physical, life/health, etc) - Morphology (root, compound, phrase) - Animacy (inanimate, animate, agentic, collaborative) - Polarity (highly negative, negative, neutral, positive, highly positive) - Idiomacity (highly literal, literal, idiomatic, highly idiomatic) - Concreteness (highly abstract, abstract, concrete, highly concrete)  Your job is to identify words/phrases that meet the requested characteristics. You respond only with the requested word or phrase in lower case.") },
            { new ChatMessage(ChatRole.User, $"Give me a word or phrase with the following characteristics:  - Primary part of speech is {target.PartOfSpeech} - Register is {target.Register} - Morphology is {target.Morphology} - Animacy is {target.Animacy} - Scientific Discipline is {target.ScientificDiscipline} - Polarity is {target.Polarity} - Idiomaticity {target.Idiomaticity} - Concreteness is {target.Concreteness}  and is not in the following list: {doNotUseText}.  Be sure to only respond with the selected word or phrase, no ceremony.") }
		};

        var response = await chatClient.GetResponseAsync(
            chatMessages,
            SlmChatOptions);

        return response.Text;
    }
}
