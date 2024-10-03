using System;
using AutoGen;
using AutoGen.Core;

namespace Solutionizer.Agents;

public class ProblemGeneratorAgent : IAgent
{
    const string _systemMessage = "You are a problem generator. You suggest problems for other agents to solve.";

    private readonly AssistantAgent _agent;

    public ProblemGeneratorAgent(LLMConfig llmConfig, string name = "Problemizer")
    {
        _agent = new AssistantAgent(
            name, 
            _systemMessage, 
            humanInputMode: HumanInputMode.NEVER,
            llmConfig: llmConfig.AsConversableAgentConfig());
    }

    public String Name => _agent.Name;

    public Task<IMessage> GenerateReplyAsync(
        IEnumerable<IMessage> messages, 
        GenerateReplyOptions? options = null, 
        CancellationToken cancellationToken = default) 
            => _agent.GenerateReplyAsync(messages, options, cancellationToken);
}
