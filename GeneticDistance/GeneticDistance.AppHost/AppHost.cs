var builder = DistributedApplication.CreateBuilder(args);

// Add the apiKey parameter from User Secrets so we
// can connect to the Qdrant dashboard from the browser.
var apiKey = builder.AddParameter("apiKey", secret: true);

// Construct the Qdrant db container with a persistent data volume
// and set its lifetime to Persistent so the container is
// not spun-down when the program ends. The container will
// download automatically if not already downloaded.
var qdrant = builder.AddQdrant("qdrant", apiKey, 6333, 6334)
    .WithEndpoint("http", endpoint =>
    {
        endpoint.IsProxied = false;
        endpoint.Port = 6333;
        endpoint.TargetPort = 6333;
    }, createIfNotExists: false)
    .WithEndpoint("grpc", endpoint =>
    {
        endpoint.IsProxied = false;
        endpoint.Port = 6334;
        endpoint.TargetPort = 6334;
    }, createIfNotExists: false)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Construct the Ollama container with a 
// data volume, and persistent lifetime.
var ollama = builder
    .AddOllama("ollama")
    .WithEndpoint("http", endpoint => endpoint.Port = 11434, createIfNotExists: false)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Add the embedding model to Ollama
// The model will be downloaded automatically if not already present.
var embeddingModel = ollama
    .AddModel("nomic-embed-text");

// Add the language model to Ollama
// The model will be downloaded automatically if not already present.
var smallLanguageModel = ollama
    .AddModel("qwen2.5:7b");

// setup the applicaton builder with:
// - a reference to the api project
// - references to the Qdrant container & Ollama container
// - references to the embedding model & language model
// - a wait for Qdrant, embedding model, and language model
//   to be ready before starting the api
var api = builder
    .AddProject<Projects.GeneticDistance_Api>("api")
	.WithEndpoint("http", endpoint => endpoint.Port = 61651, createIfNotExists: false)
    .WithReference(qdrant)
    .WithReference(ollama)
    .WithReference(embeddingModel)
    .WithReference(smallLanguageModel)
	.WaitFor(qdrant)
    .WaitFor(embeddingModel)
    .WaitFor(smallLanguageModel);

builder.Build().Run();
