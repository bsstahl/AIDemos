using GeneticDistance.Data.Qdrant;
using GeneticDistance.Domain.Interfaces;
using GeneticDistance.Execution.Options;
using GeneticDistance.Execution.Reporting;
using GeneticDistance.Execution.Services;
using Grpc.Core;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OllamaSharp;
using Qdrant.Client;

namespace GeneticDistance.CLI;

internal static class Program
{
	private static async Task<int> Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);

		builder.Services.Configure<CliDependencyOptions>(
			builder.Configuration.GetSection(CliDependencyOptions.SectionName));
		builder.Services.Configure<GeneticAlgorithmOptions>(
			builder.Configuration.GetSection(GeneticAlgorithmOptions.SectionName));

		builder.Services.AddSingleton<OllamaApiClient>(sp =>
		{
			var options = sp.GetRequiredService<IOptions<CliDependencyOptions>>().Value;
			return new OllamaApiClient(new Uri(options.OllamaEndpoint), defaultModel: "qwen2.5:7b");
		});
		builder.Services.AddSingleton<IChatClient>(sp => sp.GetRequiredService<OllamaApiClient>());
		builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp => sp.GetRequiredService<OllamaApiClient>());

		builder.Services.AddSingleton(sp =>
		{
			var options = sp.GetRequiredService<IOptions<CliDependencyOptions>>().Value;
			var apiKey = string.IsNullOrWhiteSpace(options.QdrantApiKey)
				? builder.Configuration["Parameters:apiKey"]
				: options.QdrantApiKey;

			return string.IsNullOrWhiteSpace(apiKey)
				? new QdrantClient(options.QdrantHost, options.QdrantGrpcPort)
				: new QdrantClient(options.QdrantHost, options.QdrantGrpcPort, https: false, apiKey);
		});

		builder.Services.AddSingleton<IEmbeddingRepository, Repository>();
		builder.Services.AddSingleton<IGenerationStrategy, RandomGenerationStrategy>();
		builder.Services.AddSingleton<IGeneticAlgorithmReporter, ConsoleGeneticAlgorithmReporter>();
		builder.Services.AddSingleton<GeneticAlgorithmService>();

		using var host = builder.Build();
		var dependencyOptions = host.Services.GetRequiredService<IOptions<CliDependencyOptions>>().Value;
		var service = host.Services.GetRequiredService<GeneticAlgorithmService>();

		using var cancellation = new CancellationTokenSource();
		Console.CancelKeyPress += (_, eventArgs) =>
		{
			eventArgs.Cancel = true;
			cancellation.Cancel();
		};

		try
		{
			await service.RunAsync(cancellation.Token);
			return 0;
		}
		catch (RpcException)
		{
			Console.Error.WriteLine(
				$"Unable to reach Qdrant over gRPC at {dependencyOptions.QdrantHost}:{dependencyOptions.QdrantGrpcPort}. Restart the AppHost so Qdrant is exposed on the direct proxyless port, then try the CLI again.");
			return 1;
		}
		catch (HttpRequestException)
		{
			Console.Error.WriteLine(
				$"Unable to reach Ollama at {dependencyOptions.OllamaEndpoint}. Start or restart the AppHost so the pinned Ollama endpoint is available, then try the CLI again.");
			return 1;
		}
		catch (OperationCanceledException)
		{
			Console.Error.WriteLine("Run cancelled.");
			return 1;
		}
	}
}
