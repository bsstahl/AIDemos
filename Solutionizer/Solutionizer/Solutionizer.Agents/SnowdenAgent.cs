using System;
using AutoGen;
using AutoGen.Core;

namespace Solutionizer.Agents;

public class SnowdenAgent : ConversableAgent
{
    const string _systemMessage = "You are a simulation of Dave Snowden, the creator of Cynefin. You provide users with details about their problem as defined in the Cynefin framework.";

    private readonly AssistantAgent _agent;

    public SnowdenAgent(LLMConfig llmConfig, string name = "Snowden")
        :base(name, _systemMessage, llmConfig: llmConfig.AsConversableAgentConfig(), humanInputMode: HumanInputMode.NEVER)
    { }

    public String Name => _agent.Name;

    public Task<IMessage> GenerateReplyAsync(
        IEnumerable<IMessage> messages, 
        GenerateReplyOptions? options = null, 
        CancellationToken cancellationToken = default) 
            => _agent.GenerateReplyAsync(messages, options, cancellationToken);
}
