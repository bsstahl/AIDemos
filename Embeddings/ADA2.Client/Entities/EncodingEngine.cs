using ADA2.Client.Extensions;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ADA2.Client.Entities;

public class EncodingEngine
{
    private readonly ILogger _logger;
    private readonly ModelConfig _modelConfig = new ModelConfig();
    private readonly OpenAIClientFactory _clientFactory;
    private readonly EmbeddingCache _cache;
    

    public EncodingEngine(ILogger<EncodingEngine> logger, IConfiguration config)
    {
        _logger = logger;

        config.GetSection("ModelConfig").Bind(_modelConfig);
        _clientFactory = new OpenAIClientFactory(config, _modelConfig);
        
        _cache = new EmbeddingCache();
    }

    ModelConfig CloneModelConfig()
        => _modelConfig.Clone();

    public async Task<(bool IsError, IDictionary<string, float[]>? Embeddings)> EmbedAsync(IEnumerable<string> valuesToEmbed, bool leaveNewlinesIntact = false, ModelConfig? requestConfig = null, CancellationToken? cancellationToken = null)
    {
        (bool IsError, IDictionary<string, float[]>? Embeddings) embeddingsResponse 
            = new(false, new Dictionary<string, float[]>());

        var cacheSuccess = await this.AddAllEmbeddingsToCache(valuesToEmbed, leaveNewlinesIntact, requestConfig, cancellationToken);
        if (cacheSuccess)
            valuesToEmbed.ToList().ForEach(v => embeddingsResponse.Embeddings.Add(v, _cache.TryGetValue(v).Value!));

        return embeddingsResponse;
    }

    private async Task<bool> AddAllEmbeddingsToCache(IEnumerable<string> valuesToEmbed, bool leaveNewlinesIntact = false, ModelConfig? requestConfig = null, CancellationToken? cancellationToken = null)
    {
        var result = true;

        // Get a list of all needed embeddings (not in cache)
        var values = valuesToEmbed
            .Where(v => !_cache.TryGetValue(v).ContainsKey);

        if (values.Any())
        {
            var embeddingResults = await this.GetAllEmbeddings(values, leaveNewlinesIntact, requestConfig, cancellationToken);
            if (embeddingResults.IsError)
                result = false;
            else
            {
                embeddingResults.Embeddings?.ToList().ForEach(e => _cache.Add(e.Key, e.Value));
            }
        }

        return result;
    }

    private async Task<(bool IsError, IDictionary<string, float[]>? Embeddings)> GetAllEmbeddings(IEnumerable<string> values, bool leaveNewlinesIntact = false, ModelConfig? requestConfig = null, CancellationToken? cancellationToken = null)
    {
        if (!leaveNewlinesIntact)
            values = values.Select(v => v.ReplaceLineEndings(" "));

        var modelConfig = _modelConfig.Clone();
        var config = requestConfig ?? modelConfig;
        var embeddingsOptions = config.AsEmbeddingsOptions(values);
        var token = cancellationToken ?? CancellationToken.None;

        if (token.IsCancellationRequested)
            throw new OperationCanceledException();

        (bool IsError, IDictionary<string, float[]>? Embeddings) modelResponse;
        try
        {
            OpenAIClient openAIClient = await _clientFactory.GetClientAsync()
                .ConfigureAwait(continueOnCapturedContext: false);
            modelResponse = (await openAIClient.GetEmbeddingsAsync(embeddingsOptions, cancellationToken ?? CancellationToken.None)
                .ConfigureAwait(continueOnCapturedContext: false))
                .AsEmbeddingsModelResponse(embeddingsOptions);
            _logger.LogWarning("Response received from Embeddings Model - IsError: {IsError}, Values: {@Values}", modelResponse.IsError, values);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error thrown by Embeddings Model {@Error}", ex);
            throw new InvalidOperationException("An error occurred while executing the embeddings model", ex);
        }

        return modelResponse;
    }

}
