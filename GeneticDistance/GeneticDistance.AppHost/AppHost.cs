var builder = DistributedApplication.CreateBuilder(args);

// Add the apiKey parameter from User Secrets so we
// can connect to the Qdrant dashboard from the browser.
var apiKey = builder.AddParameter("apiKey", secret: true);

// Construct the Qdrant db container with a persistent data volume
// and set its lifetime to Persistent so the container is
// not spun-down when the program ends. The container will
// download automatically if not already downloaded.
var qdrant = builder.AddQdrant("qdrant", apiKey)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Construct the Ollama container with a 
// data volume, and persistent lifetime.
var ollama = builder
    .AddOllama("ollama")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Add the embedding model to Ollama
// The model will be downloaded automatically if not already present.
var embeddingModel = ollama
    .AddModel("nomic-embed-text");

// Add the language model to Ollama
// The model will be downloaded automatically if not already present.
var smallLanguageModel = ollama
    .AddModel("llama3.2:1b");

var reasoningLanguageModel = ollama
	.AddModel("gpt-oss:20b");

// setup the applicaton builder with:
// - a reference to the api project
// - references to the Qdrant container & Ollama container
// - references to the embedding model & language model
// - a wait for Qdrant, embedding model, and language model
//   to be ready before starting the api
var api = builder
    .AddProject<Projects.GeneticDistance_Api>("api")
    .WithReference(qdrant)
    .WithReference(ollama)
    .WithReference(embeddingModel)
    .WithReference(smallLanguageModel)
    .WithReference(reasoningLanguageModel)
	.WaitFor(qdrant)
    .WaitFor(embeddingModel)
    .WaitFor(smallLanguageModel)
    .WaitFor(reasoningLanguageModel);

builder.Build().Run();
