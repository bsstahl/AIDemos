using Aspire.Hosting;
using Microsoft.Extensions.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.RagDemo_ApiService>("apiservice");

var cosmos = builder.AddAzureCosmosDB("cosmos");
var cosmosdb = cosmos
    .AddDatabase("cosmosdb")
    .RunAsEmulator();

var openAi = builder.AddAzureOpenAI("openAiConnectionName");
var qdrant = builder.AddQdrant("qdrant");

builder.AddProject<Projects.RagDemo_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiService)
    .WithReference(cosmosdb)
    .WithReference(openAi)
    .WithReference(qdrant);

builder.Build().Run();

