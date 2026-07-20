using System.Text.Json.Serialization;

namespace LinearClassification;

/// <summary>
/// A single node in the binary decision tree that forms the knowledge base.
///
/// Dual-purpose design:
///   • Internal (question) node — <see cref="Question"/> is set; <see cref="Yes"/> and
///     <see cref="No"/> point to child subtrees.
///   • Leaf (guess) node — <see cref="Guess"/> is set; <see cref="Yes"/> and
///     <see cref="No"/> are null.
///
/// This is the core data structure of the linear classifier: each question node
/// performs a binary split on one feature (the yes/no answer), and each leaf
/// represents a terminal classification (the guessed object).
/// </summary>
internal sealed class Node
{
    /// <summary>
    /// The yes/no question posed at this internal node. <see langword="null"/> for leaf nodes.
    /// Uses the C# 14 <c>field</c> keyword to trim whitespace on assignment via a semi-auto property.
    /// </summary>
    public string? Question
    {
        get => field;
        set => field = value?.Trim();
    }

    /// <summary>
    /// The object name guessed at this leaf node. <see langword="null"/> for internal nodes.
    /// Uses the C# 14 <c>field</c> keyword to trim whitespace on assignment via a semi-auto property.
    /// </summary>
    public string? Guess
    {
        get => field;
        set => field = value?.Trim();
    }

    /// <summary>Child subtree to follow when the user answers "yes". <see langword="null"/> for leaf nodes.</summary>
    public Node? Yes { get; set; }

    /// <summary>Child subtree to follow when the user answers "no". <see langword="null"/> for leaf nodes.</summary>
    public Node? No { get; set; }

    /// <summary>
    /// Returns <see langword="true"/> when this node is a leaf (contains a guess rather than a question).
    /// Excluded from JSON serialisation because it is a computed property.
    /// </summary>
    [JsonIgnore]
    public bool IsLeaf => Guess is not null;

    /// <summary>
    /// Returns the guess stored in this leaf node.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when called on a non-leaf node.</exception>
    public string GetGuess() =>
        Guess ?? throw new InvalidOperationException("GetGuess() called on a non-leaf node.");
}
