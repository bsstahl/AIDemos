using Accord.Collections;
using Beary.Embeddings.LocalServer.Extensions;
using Beary.Entities;
using Beary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KNN;

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
            .BuildServiceProvider();

        var animals = "African Elephant,African Grey Parrot,Amazon River Dolphin,Anaconda,Arctic Fox,Bald Eagle,Bengal Tiger,Black Mamba,Blue Whale,Cheetah,Clownfish,Emperor Penguin,Galapagos Tortoise,Giant Panda,Giraffe,Great White Shark,Green Sea Turtle,Indian Rhinoceros,Kangaroo,Koala,Komodo Dragon,Meerkat,Monarch Butterfly,Narwhal,Orangutan,Platypus,Polar Bear,Red Kangaroo,Sea Otter,Snow Leopard";
        var animalList = animals.Split(',');

        var key = args[0];
        int k = 4;
        bool sanitizeInputs = false;

        var program = services.GetRequiredService<Program>();
        var results = await program.KNearest(animalList, key, k, sanitizeInputs);
        var nearestAnimals = string.Join(", ", results.Select(r => $"{r.Content} ({r.Score})"));

        Console.WriteLine($"The {k} closest animals to {key} are {nearestAnimals}");
    }

    private readonly IGetEmbeddings _embeddingsClient;

    public Program(IGetEmbeddings embeddingsClient)
    {
        _embeddingsClient = embeddingsClient;
    }

    public async Task<IEnumerable<SearchResult>> KNearest(string[] data, string key, int k, bool sanitizeInputs = false)
    {
        var embeddings = await _embeddingsClient.GetEmbeddings(data, Guid.NewGuid().ToString(), sanitizeInputs);

        // Create and populate the KD-Tree
        var kdTree = new KDTree<float>(embeddings!.First()!.Embedding!.Value.Count());
        embeddings.ToList().ForEach(e => kdTree.Add(e.Embedding!.ToDoubleArray(), e.Index.Value));

        // Get the K nearest neighbors
        var keyVector = await _embeddingsClient.GetEmbedding(key, Guid.NewGuid().ToString(), sanitizeInputs);
        var results = kdTree.Nearest(keyVector!.Embedding!.ToDoubleArray(), k);

        return results
            .ToList()
            .Select(r => new SearchResult()
            {
                Id = Guid.NewGuid().ToString(),
                ElementIndex = (int)r.Node.Value,
                Content = data[(int)r.Node.Value],
                Score = r.Distance,
                Embedding = r.Node.Position.Select(p => (float)p)
            });
    }
}
