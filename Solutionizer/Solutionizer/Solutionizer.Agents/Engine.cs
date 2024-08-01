using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;

namespace Solutionizer.Agents;

public class Engine
{

    public async Task Execute()
    {
        var openAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new Exception("Please set OPENAI_API_KEY environment variable.");
        var gpt35Config = new OpenAIConfig(openAIKey, "gpt-3.5-turbo");

        var assistantAgent = new AssistantAgent(
            name: "assistant",
            systemMessage: "You are an assistant that help user to do some tasks.",
            llmConfig: new ConversableAgentConfig
            {
                Temperature = 0,
                ConfigList = [gpt35Config],
            })
            .RegisterPrintMessage(); // register a hook to print message nicely to console

        // set human input mode to ALWAYS so that user always provide input
        var userProxyAgent = new UserProxyAgent(
            name: "user",
            humanInputMode: HumanInputMode.ALWAYS)
            .RegisterPrintMessage();

        // start the conversation
        await userProxyAgent.InitiateChatAsync(
            receiver: assistantAgent,
            message: "Hey assistant, please do me a favor.",
            maxRound: 10);
    }
}
