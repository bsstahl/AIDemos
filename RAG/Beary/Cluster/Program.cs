using Accord.MachineLearning;
using Beary.Chat.AzureGpt.Extensions;
using Beary.Chat.Entities;
using Beary.Chat.Interfaces;
using Beary.Data.AzureAISearch.Extensions;
using Beary.Data.Interfaces;
using Beary.Embeddings.LocalServer.Extensions;
using Beary.ValueTypes;
using Cluster.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using static Azure.Core.HttpHeader;

namespace Cluster;

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
            .UseLocalServerEmbeddingsModel()
            .UseAzureAIEmbeddingsReadRepo()
            .UseAzureGptChatClient()
            .BuildServiceProvider();

        var program = services.GetRequiredService<Program>();
        await program.GetParagraphClusters();
    }

    private readonly IReadEmbeddingsSearchDocuments _embeddingsReadRepo;
    private readonly ICreateChatCompletions _chatClient;

    public Program(IReadEmbeddingsSearchDocuments embeddingsReadRepo, ICreateChatCompletions chatClient)
    {
        _embeddingsReadRepo = embeddingsReadRepo;
        _chatClient = chatClient;
    }

    internal async Task GetParagraphClusters()
    {
        const string userPrompt = "In one sentence, describe the common characteristics of the following paragraphs?";
        const string systemPrompt = "You are an expert at categorizing the paragraphs of multiple related documents, highlighting key themes and information while avoiding redundancy. You analyze content for main ideas, synthesize common themes, ensure logical structure, and balance brevity with completeness.";

        // Fetch all embeddings from the Azure AI Search service
        var embeddings = await _embeddingsReadRepo.GetAllEmbeddings();

        // Calculate the Clusters

        int k = 10;
        var kmeans = new KMeans(k);
        var clusters = kmeans.Learn(embeddings.AsEmbeddingsArray());

        foreach (var cluster in clusters)
        {
            var content = new List<ChatContent>()
            {
                ChatContent.From(systemPrompt, ChatRole.System)
            };

            var clusterCount = Convert.ToInt32(Math.Floor(cluster.Proportion * Convert.ToDouble(embeddings.Count())));
            var sampleCount = Math.Min(clusterCount, 5);
            var samples = await _embeddingsReadRepo.GetNearestNeighbors(cluster.CentroidVector(), ResultCount.From(sampleCount));
            
            content.AddRange(samples.AsChatContents(ChatRole.User));
            content.Add(ChatContent.From(userPrompt, ChatRole.User));

            var result = await _chatClient.CreateChatCompletionsAsync(content);
            
            result.OutputToUser();
        }
    }

    internal async Task GetArticleClusters()
    {
        // Fetch all embeddings from the Azure AI Search service
        var embeddings = await _embeddingsReadRepo.GetAllEmbeddings();

        // Calculate the median embedding for each article
        // Calculate the Clusters on the article medians
        // Get a bullet-point list of the Clusters
        // Output the bullet-point list to the user

        throw new NotImplementedException();
    }
}
