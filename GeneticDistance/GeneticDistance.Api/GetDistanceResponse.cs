using GeneticDistance.Domain.Entities;

namespace GeneticDistance.Api;

public class GetDistanceResponse(Expression target, float distanceFromSource)
{
	public Expression Target { get; set; } = target;

	public float DistanceFromSource { get; set; } = distanceFromSource;
}
