namespace KMeans;

public sealed class KMeansResult(
        IReadOnlyList<float[]> centroids,
        IReadOnlyDictionary<string, int> assignmentsById)
{
    public IReadOnlyList<float[]> Centroids { get; } = centroids;
    public IReadOnlyDictionary<string, int> AssignmentsById { get; } = assignmentsById;

}