using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Clustering;

public static class ClusterExtensions
{
    public static string Serialize(this IEnumerable<Cluster> clusters)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();
        return serializer.Serialize(clusters);
    }

}
