using Qdrant.Client.Grpc;

namespace AxiomVectorRepository;

internal static class PointExtensions
{
    internal static Axiom AsAxiom(this RetrievedPoint point)
    {
        return new Axiom(
            point.Id.Uuid.ToString(),
            point.Payload["text"].StringValue,
            point.Vectors.Vector.Data.ToArray());
    }

    internal static Axiom AsAxiom(this ScoredPoint point)
    {
        return new Axiom(
            point.Id.Uuid.ToString(),
            point.Payload["text"].StringValue,
            point.Vectors.Vector.Data.ToArray(),
            point.Score);
    }
}
