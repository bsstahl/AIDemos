using Beary.Chat.Entities;
using Beary.ValueTypes;

namespace Cluster.Extensions;

internal static class KMeansClusterExtensions
{
    internal static Vector CentroidVector(this Accord.MachineLearning.KMeansClusterCollection.KMeansCluster cluster)
    {
        return Vector.From(cluster.Centroid.Select(c => Convert.ToSingle(c)));
    }
}
