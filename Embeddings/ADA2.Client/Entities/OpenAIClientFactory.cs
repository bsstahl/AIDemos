using ADA2.Client.Extensions;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

namespace ADA2.Client.Entities;

internal sealed class OpenAIClientFactory
{
    private readonly ModelConfig _modelConfig;
    private readonly OpenAIClientOptions _options;
    
    private string _clientKey = string.Empty;

    private OpenAIClient? _currentClient;

    public ModelConfig ModelConfig => _modelConfig;
    public OpenAIClientOptions Options => _options;


    public OpenAIClientFactory(IConfiguration config, ModelConfig modelConfig)
    {
        _modelConfig = modelConfig;
        _options = new OpenAIClientOptions();
        _clientKey = config["OpenAIClientKey"] ?? string.Empty;
    }

    public Task<OpenAIClient> GetClientAsync()
    {
        if (string.IsNullOrEmpty(_clientKey))
            throw new AccessViolationException("A valid client key must be supplied");

        if (_currentClient is null)
        {
            var cred = new Azure.AzureKeyCredential(_clientKey);
            _currentClient = new OpenAIClient(_modelConfig.SemanticServiceUrl.AsUri(), cred, _options);
        }

        return Task.FromResult(_currentClient);
    }
}
