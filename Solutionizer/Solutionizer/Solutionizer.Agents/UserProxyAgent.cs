using System;
using AutoGen;
using AutoGen.Core;

namespace Solutionizer.Agents;

public class UserAgent : IAgent
{
    const string _systemMessage = "You are a problem generator. You suggest problems for other agents to solve.";

    private readonly UserProxyAgent _agent;

    public UserAgent(LLMConfig llmConfig, string name = "User")
    {
        _agent = new UserProxyAgent(
            name, 
            _systemMessage, 
            humanInputMode: HumanInputMode.ALWAYS,
            llmConfig: llmConfig.AsConversableAgentConfig());
    }

    public String Name => _agent.Name;

    public Task<IMessage> GenerateReplyAsync(
        IEnumerable<IMessage> messages, 
        GenerateReplyOptions? options = null, 
        CancellationToken cancellationToken = default) 
            => _agent.GenerateReplyAsync(messages, options, cancellationToken);
}
