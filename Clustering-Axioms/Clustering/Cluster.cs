using YamlDotNet.Serialization;

namespace Clustering;

public class Cluster
{
    public int Id { get; set; }
    public string NearestNodeId { get; set; }
    public string Label { get; set; }
    public IEnumerable<KeyValuePair<string, string>> Members { get; set; }

    [YamlIgnore]
    public float[] Centroid { get; set; }


    public Cluster(int id, float[] centroid, string nearestNodeId, string label, IEnumerable<KeyValuePair<string, string>> members)
    {
        this.Id = id;
        this.Centroid = centroid;
        this.NearestNodeId = nearestNodeId;
        this.Label = label;
        this.Members = members;
    }

    override public string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"GroupId: {this.Id}");
        sb.AppendLine($"Label: {this.Label}");
        sb.AppendLine($"NearestNodeID: {this.NearestNodeId}");
        sb.AppendLine($"MemberCount: {this.Members.Count()}");
        sb.AppendLine("Members:");
        foreach (var member in this.Members)
            sb.AppendLine($"- {member.Value}");
        sb.AppendLine();
        return sb.ToString();
    }

}
