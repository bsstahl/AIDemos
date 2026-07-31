using GeneticDistance.Execution;
using GeneticDistance.Execution.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeneticDistance.Api.Controllers;

[ApiController]
[Route("genetic-algorithm")]
public sealed class GeneticAlgorithmController : ControllerBase
{
	private readonly GeneticAlgorithmService _service;

	public GeneticAlgorithmController(GeneticAlgorithmService service)
	{
		_service = service;
	}

	[HttpPost("run")]
	public Task<GeneticAlgorithmRunResult> Run(CancellationToken cancellationToken)
		=> _service.RunAsync(cancellationToken);
}
