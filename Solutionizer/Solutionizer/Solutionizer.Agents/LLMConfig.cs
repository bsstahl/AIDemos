using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using Microsoft.Extensions.Configuration;

namespace Solutionizer.Agents;

public class LLMConfig : ILLMConfig
{
    public string ApiKey { get; }
    public string Endpoint { get; }
    public string ModelId { get; }
    public string DeploymentName => this.ModelId;

    public LLMConfig(IConfiguration config)
        : this(
              config?["AzureOpenAI:ApiKey"] ?? string.Empty,
              config?["AzureOpenAI:Endpoint"] ?? string.Empty, 
              "gpt-4o")
    { }

    public LLMConfig(string apiKey, string endpoint, string modelId)
    {
        this.ApiKey = apiKey;
        this.Endpoint = endpoint;
        this.ModelId = modelId;
    }

    public ConversableAgentConfig AsConversableAgentConfig()
    {
        return new ConversableAgentConfig
        {
            Temperature = 0,
            ConfigList = new[] { new AzureOpenAIConfig(this.Endpoint, this.DeploymentName, this.ApiKey, this.ModelId) }
        };
    }
}
