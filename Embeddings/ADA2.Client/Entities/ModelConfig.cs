namespace ADA2.Client.Entities;

using System.Collections.Generic;
using System.Linq;

public class ModelConfig
{
    public string ModelName { get; set; }

    public uint MaxTokens { get; set; }

    public float Temperature { get; set; }

    public float NucleusSamplingFactor { get; set; }

    public uint ChoiceCount { get; set; }

    public IEnumerable<string> StopSequences { get; set; }

    public float PresencePenalty { get; set; }

    public float FrequencyPenalty { get; set; }

    public IDictionary<int, int> TokenSelectionBiases { get; set; }

    public string SemanticServiceUrl { get; set; }

    public ModelConfig()
        : this("gpt-35-turbo", 1000, 0.0f, 1.0f, 1u, Enumerable.Empty<string>(), 0.0f, 0.0f, new Dictionary<int, int>(), "http://example.com")
    { }

    public ModelConfig(string modelName, uint maxTokens, float temperature, float nucleusSamplingFactor, uint choiceCount, IEnumerable<string> stopSequences, float presencePenalty, float frequencyPenalty, IDictionary<int, int> tokenSelectionBiases, string semanticServiceUrl)
    {
        ModelName = modelName;
        MaxTokens = maxTokens;
        Temperature = temperature;
        NucleusSamplingFactor = nucleusSamplingFactor;
        ChoiceCount = choiceCount;
        StopSequences = stopSequences;
        PresencePenalty = presencePenalty;
        FrequencyPenalty = frequencyPenalty;
        TokenSelectionBiases = tokenSelectionBiases;
        SemanticServiceUrl = semanticServiceUrl;
    }

    public ModelConfig Clone()
    {
        return new ModelConfig(ModelName, MaxTokens, Temperature, NucleusSamplingFactor, ChoiceCount, StopSequences, PresencePenalty, FrequencyPenalty, TokenSelectionBiases, SemanticServiceUrl);
    }
}
