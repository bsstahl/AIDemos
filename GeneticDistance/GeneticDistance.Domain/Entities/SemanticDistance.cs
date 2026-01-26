namespace GeneticDistance.Domain.Entities;

public class SemanticDistance(Expression pointA, Expression pointB, float distance)
{
	public Expression PointA { get; set; } = pointA;
	public Expression PointB { get; set; } = pointB;

	public float Distance { get; set; } = distance;
}
