namespace LinearClassification;

/// <summary>
/// Implements the 20 Questions game loop backed by a binary decision tree
/// (a simple linear / binary classifier).
///
/// Each internal node in the tree is one binary feature question.
/// Traversal from root to leaf is a sequence of feature evaluations that
/// ultimately classifies the object the user is thinking of.
/// </summary>
internal sealed class GameEngine
{
    private readonly TreeRepository _repository;

    // The root may be replaced when the tree consists of a single leaf
    // and the first learning event occurs, so it is stored as a field.
    private Node _root;

    public GameEngine(TreeRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
        _root = repository.Load();
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Runs one complete round of 20 Questions.</summary>
    public void PlayRound()
    {
        ConsoleUI.Blank();
        ConsoleUI.Say("Think of any object — I'll try to guess it by asking yes/no questions.");
        ConsoleUI.Blank();

        // ── Traverse the tree ─────────────────────────────────────────────
        // We keep a reference to the parent and the direction we took so that
        // the learning step knows exactly where to graft the new question node.
        Node?  parent        = null;
        bool   parentTookYes = false;
        Node   current       = _root;

        while (!current.IsLeaf)
        {
            bool answer = ConsoleUI.AskYesNo(current.Question!);
            parent        = current;
            parentTookYes = answer;
            current = answer
                ? current.Yes ?? throw new InvalidOperationException("Malformed tree: Yes branch is null.")
                : current.No  ?? throw new InvalidOperationException("Malformed tree: No branch is null.");
        }

        // ── Make the guess ────────────────────────────────────────────────
        ConsoleUI.Blank();
        ConsoleUI.Say($"My guess: \"{current.GetGuess()}\"");
        bool correct = ConsoleUI.AskYesNo("Was I right?");

        if (correct)
        {
            ConsoleUI.Say("✓ Got it! My classifier is working perfectly.");
            return;
        }

        // ── Learn from the mistake ────────────────────────────────────────
        LearnFromMistake(current, parent, parentTookYes);
        _repository.Save(_root);
        ConsoleUI.Say("✓ Knowledge base updated — I won't make that mistake again!");
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Expands the tree at <paramref name="incorrectLeaf"/> by inserting a new
    /// question node that distinguishes the user's object from the wrong guess.
    ///
    /// Tree structure before learning:
    ///   [parent] ──(yes|no)──▶ [incorrectLeaf]
    ///
    /// Tree structure after learning:
    ///   [parent] ──(yes|no)──▶ [newQuestionNode]
    ///                               ├── yes ──▶ [A]
    ///                               └── no  ──▶ [B]
    ///
    /// Where [A] and [B] are the new-object leaf and the old incorrect-guess
    /// leaf, assigned to the yes/no branches according to the user's answer.
    /// </summary>
    private void LearnFromMistake(Node incorrectLeaf, Node? parent, bool parentTookYes)
    {
        ConsoleUI.Blank();
        string newObjectName = ConsoleUI.AskString(
            "I give up! What were you thinking of?");

        string question = ConsoleUI.AskString(
            $"Give me a yes/no question that distinguishes \"{newObjectName}\" from \"{incorrectLeaf.GetGuess()}\"");

        // Ensure the question ends with a question mark for nicer output
        if (!question.EndsWith('?'))
            question += "?";

        bool newObjectIsYes = ConsoleUI.AskYesNo(
            $"For \"{newObjectName}\", is the answer to \"{question}\" yes?");

        // Build the replacement question node
        var newObjectLeaf = new Node { Guess = newObjectName };
        var newQuestionNode = new Node
        {
            Question = question,
            Yes = newObjectIsYes ? newObjectLeaf : incorrectLeaf,
            No  = newObjectIsYes ? incorrectLeaf : newObjectLeaf,
        };

        // Attach the new question node to the correct position in the tree.
        // Three cases:
        //   1. The entire tree was a single leaf (root) — replace the root.
        //   2. The leaf was the yes-child of its parent — update parent.Yes.
        //   3. The leaf was the no-child of its parent  — update parent.No.
        if (parent is null)
        {
            _root = newQuestionNode;
        }
        else if (parentTookYes)
        {
            parent.Yes = newQuestionNode;
        }
        else
        {
            parent.No = newQuestionNode;
        }
    }
}
