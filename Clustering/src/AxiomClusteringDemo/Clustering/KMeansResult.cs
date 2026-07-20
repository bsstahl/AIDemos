namespace AxiomClusteringDemo.Clustering;

internal sealed record KMeansResult(
    IReadOnlyList<int> Assignments,
    IReadOnlyList<IReadOnlyList<float>> Centroids,
    int Iterations,
    bool Converged);
