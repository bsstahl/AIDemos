namespace KMeans;

public interface IClusterVectors
{
    Task<KMeansResult> Fit(IReadOnlyList<VectorPoint> points, Int32 k, Int32 maxIterations = 100, Single tolerance = 0.0001F);
    Task<KMeansResult> Fit(IReadOnlyList<VectorPoint> points, Int32 minK, Int32 maxK, Int32 maxIterations = 100, Single tolerance = 0.0001F);
    Task<Single> GetSilhouetteScore(IReadOnlyList<VectorPoint> points, KMeansResult result);
}