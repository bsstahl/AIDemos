using GeneticDistance.Data.Qdrant;
using GeneticDistance.Domain.Interfaces;
using GeneticDistance.Execution.Options;
using GeneticDistance.Execution.Reporting;
using GeneticDistance.Execution.Services;

namespace GeneticDistance.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddControllers();

        // Add Qdrant Client
        builder
            .AddQdrantClient("qdrant");

        // Add Ollama Api Client
        var ollamaApiClient = builder
            .AddOllamaApiClient("ollama");

        // Add Ollama Chat Client
        ollamaApiClient.AddChatClient();

        // Add Ollama Embeddings Generator
        ollamaApiClient.AddEmbeddingGenerator();

        builder.Services.Configure<GeneticAlgorithmOptions>(
            builder.Configuration.GetSection(GeneticAlgorithmOptions.SectionName));
        builder.Services.AddScoped<IEmbeddingRepository, Repository>();
        builder.Services.AddScoped<IGenerationStrategy, RandomGenerationStrategy>();
        builder.Services.AddScoped<GeneticAlgorithmService>();
        builder.Services.AddSingleton<IGeneticAlgorithmReporter, NullGeneticAlgorithmReporter>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapDefaultEndpoints();
        app.MapControllers();
        app.Run();
    }
}
