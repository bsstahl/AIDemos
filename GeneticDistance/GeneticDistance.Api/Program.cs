namespace GeneticDistance.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
