using System.Text.Json;

namespace LinearClassification;

/// <summary>
/// Handles loading the decision tree from disk and persisting it back to JSON.
/// The file is written after every learning event so progress is never lost.
/// </summary>
internal sealed class TreeRepository
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new() { WriteIndented = true };

    private readonly string _filePath;

    /// <param name="filePath">Path to the JSON knowledge-base file.</param>
    public TreeRepository(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    /// <summary>
    /// Loads the tree from <see cref="_filePath"/>.
    /// When the file does not yet exist the minimal seed tree is returned:
    /// <code>
    ///   Is it an animal?
    ///   ├── yes → cat
    ///   └── no  → rock
    /// </code>
    /// </summary>
    public Node Load()
    {
        if (!File.Exists(_filePath))
            return CreateSeedTree();

        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<Node>(json) ?? CreateSeedTree();
    }

    /// <summary>Returns the two-object seed tree used when no knowledge base exists yet.</summary>
    private static Node CreateSeedTree() => new()
    {
        Question = "Is it an animal?",
        Yes = new Node { Guess = "cat" },
        No  = new Node { Guess = "rock" },
    };

    /// <summary>Serialises <paramref name="root"/> and writes it to <see cref="_filePath"/>.</summary>
    public void Save(Node root)
    {
        ArgumentNullException.ThrowIfNull(root);
        string json = JsonSerializer.Serialize(root, SerializerOptions);
        File.WriteAllText(_filePath, json);
    }
}
