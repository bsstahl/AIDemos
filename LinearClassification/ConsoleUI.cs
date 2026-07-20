namespace LinearClassification;

/// <summary>
/// Provides all console input/output helpers used by the game.
/// Centralising I/O here makes the game logic easy to unit-test in isolation.
/// </summary>
internal static class ConsoleUI
{
    // ── Output ───────────────────────────────────────────────────────────────

    /// <summary>Writes <paramref name="message"/> to stdout followed by a newline.</summary>
    public static void Say(string message) => Console.WriteLine(message);

    /// <summary>Writes a blank line to stdout.</summary>
    public static void Blank() => Console.WriteLine();

    // ── Input ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Prompts the user with <paramref name="question"/> and loops until a
    /// recognisable yes/no answer is entered.
    /// </summary>
    /// <returns><see langword="true"/> for yes, <see langword="false"/> for no.</returns>
    public static bool AskYesNo(string question)
    {
        while (true)
        {
            Console.Write($"{question} (yes/no): ");
            string? input = Console.ReadLine()?.Trim().ToUpperInvariant();

            switch (input)
            {
                case "YES" or "Y": return true;
                case "NO"  or "N": return false;
                default:
                    Console.WriteLine("  → Please answer 'yes' or 'no'.");
                    break;
            }
        }
    }

    /// <summary>
    /// Prompts the user with <paramref name="prompt"/> and loops until a
    /// non-empty string is entered.
    /// </summary>
    public static string AskString(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            string? input = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.WriteLine("  → Please enter a non-empty response.");
        }
    }
}
