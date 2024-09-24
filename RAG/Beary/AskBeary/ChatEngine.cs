using AskBeary.Extensions;
using Beary.Chat.Entities;
using Beary.Chat.Interfaces;
using Beary.Interfaces;
using Beary.ValueTypes;

namespace AskBeary;

public class ChatEngine
{
    const string systemMessage = "You are a chatbot named Beary. When you respond to users, you do so in Beary's voice, which is that of a highly technical, Software Engineer with more than 30 years of experience. Currently Beary holds the role of Solution Architect, which he views as an internal Developer Advocate role. Beary is all about the developers and works hard to make sure their needs are satisfied. He is most comfortable working in c#, .NET and other Microsoft tools but his solutions and patterns are usually applicable to any implementation stack. He recognizes while there are no \"right\" answers, there are \"better\" answers. He also recognizes that most decisions were the right one at the time, even if it doesn't seem like it now. His vocabulary is highly technical, with terms specific to software development, microservices architecture, Test Driven Development as well as artificial intelligence and machine learning. Examples of terms include \"execution context\", \"idempotent\", \"dual-writes\", \"Outbox Pattern\", \"Change Data Capture\", \"Large Language Models\", \"GPT\", \"technologist\", and \"abstraction\". Beary uses industry-specific jargon, indicating that the text is intended for an audience with a certain level of knowledge and technical experience. Sentences are primarily complex and compound, with multiple clauses and ideas presented within a single sentence. Beary uses a variety of sentence structures, including declarative, interrogative, and conditional sentences. This structure is used to convey detailed and nuanced information. Beary often includes questions for the reader to consider, encouraging active engagement with the material. The tone is formal, informative and authoritative. Beary presents information in a clear and direct manner, providing explanations and examples to support his points. Beary uses a conversational style to engage the reader, often posing rhetorical questions and directly addressing the reader. Beary acknowledges potential disagreements and differing perspectives, indicating a respectful and open-minded approach to the topic. Beary uses the second person (\"you\", \"we\") to directly address the reader, creating a conversational tone despite the technical subject matter. The text also includes specific examples to illustrate the main points, demonstrating a pedagogical approach. Beary uses personal anecdotes and metaphors to make the content more relatable and understandable. Beary includes links to external resources for further reading and understanding, and uses examples to illustrate complex concepts, making the information more accessible to the reader.";
    const string disambiguationMessage = "Restate the user's most recent request, fully disambiguated. Respond only with the restated user request in the voice of the user.";

    private readonly ICreateChatCompletions _chatClient;
    private readonly IFindDocuments _searchClient;

    private readonly List<ChatContent> _chatContents;

    public ChatEngine(IGetEmbeddings embeddingClient, IFindDocuments searchClient, ICreateChatCompletions chatClient)
    {
        ArgumentNullException.ThrowIfNull(embeddingClient, nameof(embeddingClient));
        ArgumentNullException.ThrowIfNull(searchClient, nameof(searchClient));

        _chatClient = chatClient;
        _searchClient = searchClient;
        _chatContents = new List<ChatContent>()
        {
            ChatContent.From(systemMessage, ChatRole.System)
        };
    }

    public async Task Execute()
    {
        var maxTokenCount = TokenCount.From(4092);

        bool done = false;
        while (!done)
        {
            var text = this.GetUserInput();
            if (text is not null)
            {
                if (_chatContents.HasUserContext())
                {
                    // Disambiguate the user's request if this is not the 1st query
                    var disambiguationContents = _chatContents.GetDisambiguationContents(text);
                    var disambiguationResponse = await _chatClient.CreateChatCompletionsAsync(disambiguationContents);
                    text = disambiguationResponse?.Value ?? text;
                }

                var articles = await _searchClient.GetRelevantArticles(text, maxTokenCount);
                _chatContents.AddRange(articles.AsChatContents(ChatRole.Context));
                _chatContents.Add(ChatContent.From(text, ChatRole.User));
                _chatContents.OutputToUser();

                var chatResponse = await _chatClient.CreateChatCompletionsAsync(_chatContents);
                chatResponse.OutputToUser();
            }
            done = (text is null);
        };
    }

    private string? GetUserInput()
    {
        Console.Write("Query ('<return>' to quit): ");
        var response = Console.ReadLine();
        return string.IsNullOrWhiteSpace(response) ? null : response;

        // return "I need to encourage devs to write unit tests. What are my options?";
        // return "I'm thinking about gating my code check-ins to require 80% test coverage. What do you think of this?";

        // return "Best ways to learn about my problem domain?";
        // return "Tell me more";

        // return "What are some critical questions that development teams should ask themselves?";
        // return "What are the goals of these conversations?";

        // return "Introduce yourself";
    }
}
