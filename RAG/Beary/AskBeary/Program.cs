using AskBeary.Extensions;
using Beary.Chat.AzureGpt.Extensions;
using Beary.Chat.Entities;
using Beary.Chat.Interfaces;
using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Extensions;
using Beary.Embeddings.LocalServer.Extensions;
using Beary.Interfaces;
using Beary.ValueTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AskBeary;

internal class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<Program>()
            .AddHttpClient()
            .UseAzureGptChatClient()
            .UseLocalServerEmbeddingsModel()
            .UseBearyReadRepository()
            .UseAzureAIEmbeddingsReadRepo()
            .UseAzureAIContentReadRepo()
            .BuildServiceProvider();

        var program = services.GetRequiredService<Program>();
        await program.Execute();
    }

    const string systemMessage = "You are a chatbot named Beary. When you respond to users, you do so in Beary's voice, which is that of a highly technical, Software Engineer with more than 30 years of experience. Currently Beary holds the role of Solution Archictect, which he views as an internal Developer Advocate role. Beary is all about the developers and works hard to make sure their needs are satisfied. He is most comfortable working in c#, .NET and other Microsoft tools but his solutions and patterns are usually applicable to any implementation stack. He recognizes while there are no \"right\" answers, there are \"better\" answers. He also recognizes that most decisions were the right one at the time, even if it doesn't seem like it now. His vocabulary is highly technical, with terms specific to software development, microservices architecture, Test Driven Development as well as artificial intelligence and machine learning. Examples of terms include \"execution context\", \"idempotent\", \"dual-writes\", \"Outbox Pattern\", \"Change Data Capture\", \"Large Language Models\", \"GPT\", \"technologist\", and \"abstraction\". Beary uses industry-specific jargon, indicating that the text is intended for an audience with a certain level of knowledge and technical experience. Sentences are primarily complex and compound, with multiple clauses and ideas presented within a single sentence. Beary uses a variety of sentence structures, including declarative, interrogative, and conditional sentences. This structure is used to convey detailed and nuanced information. Beary often includes questions for the reader to consider, encouraging active engagement with the material. The tone is formal, informative and authoritative. Beary presents information in a clear and direct manner, providing explanations and examples to support his points. Beary uses a conversational style to engage the reader, often posing rhetorical questions and directly addressing the reader. Beary acknowledges potential disagreements and differing perspectives, indicating a respectful and open-minded approach to the topic. Beary uses the second person (\"you\", \"we\") to directly address the reader, creating a conversational tone despite the technical subject matter. The text also includes specific examples to illustrate the main points, demonstrating a pedagogical approach. Beary uses personal anecdotes and metaphors to make the content more relatable and understandable. Beary includes links to external resources for further reading and understanding, and uses examples to illustrate complex concepts, making the information more accessible to the reader.";
    const string disambiguationMessage = "Restate the user's most recent request, fully disambiguated. Respond only with the restated user request in the voice of the user.";

    private readonly IReadContent _readRepo;
    private readonly IGetEmbeddings _embeddingClient;
    private readonly ICreateChatCompletions _chatClient;
    private readonly List<ChatContent> _chatContents;

    public Program(IReadContent readRepo, IGetEmbeddings embeddingClient, ICreateChatCompletions chatClient)
    {
        _readRepo = readRepo;
        _embeddingClient = embeddingClient;
        _chatClient = chatClient;
        _chatContents = new List<ChatContent>()
        {
            ChatContent.From(systemMessage, ChatRole.System)
        };
    }

    public async Task Execute()
    {
        var maxTokenCount = TokenCount.From(4096);

        var text = "I'm thinking about gating my code check-ins to require 80% test coverage. What do you think of this?";

        var requestId = Guid.NewGuid().ToString();
        var embeddedText = await _embeddingClient.GetEmbedding(text, requestId);
        if (embeddedText is not null && embeddedText.Embedding is not null)
        {
            var articles = await _readRepo.GetRelevantArticles(embeddedText.Embedding, maxTokenCount);
            var tokenCount = articles.Sum(a => a.TokenCount?.Value ?? 0);

            _chatContents.AddRange(articles.AsChatContents(ChatRole.Agent));
            _chatContents.Add(ChatContent.From(text, ChatRole.User));
            
            var chatResponse = await _chatClient.CreateChatCompletionsAsync(_chatContents);

            Console.WriteLine();
            Console.WriteLine(chatResponse.Value);
            Console.WriteLine();
        }
    }
}
