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
using System.Text;
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
        const int maxSampleParagraphs = 300;
        const int k = 4;

        // Fetch all embeddings from the Azure AI Search service
        var embeddings = await _embeddingsReadRepo.GetAllEmbeddings();

        // Calculate the Clusters

        var kmeans = new KMeans(k);
        var clusters = kmeans.Learn(embeddings.AsEmbeddingsArray());
        var samplesPerCluster = maxSampleParagraphs / k;

        var allResults = new List<string>();
        foreach (var cluster in clusters)
        {
            var content = new List<ChatContent>()
            {
                ChatContent.From(systemPrompt, ChatRole.System)
            };

            int clusterActualCount = Convert.ToInt32(Math.Floor(cluster.Proportion * Convert.ToDouble(embeddings.Count())));
            var maxSampleCount = Math.Min(ResultCount.MaxValue, Math.Min(clusterActualCount, Math.Max(samplesPerCluster, 5)));
            var samples = await _embeddingsReadRepo.GetNearestNeighbors(cluster.CentroidVector(), ResultCount.From(maxSampleCount));

            content.AddRange(samples.AsChatContents(ChatRole.User));
            content.Add(ChatContent.From(userPrompt, ChatRole.User));

            var result = await _chatClient.CreateChatCompletionsAsync(content);
            allResults.Add($"\"{result.Value.Replace("\"", "\\\"")}\"");

            result.OutputToUser(maxSampleCount, clusterActualCount);
        }

        var path = $"c:\\s\\Temp\\BlogClusters\\{Path.GetRandomFileName()}.csv";
        File.WriteAllText(path, string.Join("\r\n", allResults));
        Console.WriteLine($"Output written to: {path}");
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
