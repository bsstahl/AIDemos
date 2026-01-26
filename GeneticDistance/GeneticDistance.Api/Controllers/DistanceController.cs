using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using GeneticDistance.Api.Extensions;
using GeneticDistance.Domain.Entities;
using GeneticDistance.Domain.ValueTypes;

namespace GeneticDistance.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DistanceController : ControllerBase
{
    private readonly Qdrant.Client.QdrantClient _dbClient;
    private readonly IChatClient _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingsClient;

    public DistanceController(
		Qdrant.Client.QdrantClient dbClient,
        IChatClient chatClient,
        IEmbeddingGenerator<string, Embedding<float>> embeddingsClient)
    {
        _dbClient = dbClient;
        _chatClient = chatClient;
        _embeddingsClient = embeddingsClient;
    }

    [HttpPost(Name = "GetDistantExpression")]
    public async Task<GetDistanceResponse> GetDistantExpression(GetDistanceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentNullException.ThrowIfNull(request.TargetCharacteristics, nameof(request.TargetCharacteristics));
        ArgumentNullException.ThrowIfNull(request.SourceVector, nameof(request.SourceVector));
        
        var sourceVector = request.SourceVector;

        var exclusions = request
            .AdditionalExclusions
            .ToList();
        exclusions.Add(request.SourceText);

		var candidate = await _chatClient.GetCandidateAsync(request.TargetCharacteristics, exclusions.Distinct());
        // TODO: Validate result

		var embedding = await _embeddingsClient.GetEmbeddingAsync(candidate);
        var targetVector = Vector.From(embedding.ToArray());
        var targetExpressionId = Guid.NewGuid().ToString();
        var targetExpression = new Expression(targetExpressionId, candidate, targetVector, request.TargetCharacteristics);

		float distance = sourceVector.CosineDistanceFrom(targetVector);
        // TODO: Publish results for additional downstream processing

		return new GetDistanceResponse(targetExpression, distance);
    }
}
