using AutoGen;
using AutoGen.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Agents;

namespace Solutionizer.Agents;

public class Engine
{
    private readonly IConfiguration _config;

    public Engine(IConfiguration config)
    {
        _config = config;
    }


    public async Task Execute()
    {
        // TODO: Ask the user to describe the problem
        // TODO: Ask the Snowden agent for the type of problem
        // TODO: Determine which agent to use to solve the problem
        // TODO: Ask the solver agent to determine the path to solve the problem
        // TODO: Put the user on the path

        var gptConfig = new LLMConfig(_config);

        var userProxyAgent = new UserAgent(gptConfig)
            .RegisterPrintMessage();

        var problemGeneratorAgent = new ProblemGeneratorAgent(gptConfig)
            .RegisterPrintMessage();

        var snowdenAgent = new SnowdenAgent(gptConfig);
            // .RegisterPrintMessage();

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        var groupChat = new AgentGroupChat(snowdenAgent, userProxyAgent, problemGeneratorAgent);
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        //var assistantAgent = new AssistantAgent(
        //    name: "helper",
        //    systemMessage: "You help to solve problems by walking us through the appropriate steps 1 at a time.",
        //    llmConfig: gptConfig.AsConversableAgentConfig())
        //        .RegisterPrintMessage(); // register a hook to print message nicely to console

        // start the conversation
        await snowdenAgent.InitiateChatAsync(
            receiver: problemGeneratorAgent,
            message: "What is our problem today?",
            maxRound: 5);
    }
}
