// ┌────────────────────────────────────────────────────────────────────────────┐
// │  20 Questions — Linear Classifier Demo                                     │
// │                                                                            │
// │  This program demonstrates a binary decision tree used as a simple linear  │
// │  (binary) classifier.  Each internal node poses one yes/no question        │
// │  (a single binary feature split), and each leaf is a classification         │
// │  result (the guessed object).                                               │
// │                                                                            │
// │  The tree is persisted to "knowledge.json" in the working directory and    │
// │  grows automatically as you play — the more you play, the smarter it gets. │
// └────────────────────────────────────────────────────────────────────────────┘

using LinearClassification;

PrintBanner();

var repository = new TreeRepository("knowledge.json");
var engine     = new GameEngine(repository);

do
{
    engine.PlayRound();
    ConsoleUI.Blank();
}
while (ConsoleUI.AskYesNo("Play again?"));

ConsoleUI.Blank();
ConsoleUI.Say("Thanks for playing — knowledge base saved to knowledge.json. Goodbye!");

// ── Helpers ─────────────────────────────────────────────────────────────────

static void PrintBanner()
{
    ConsoleUI.Say("╔══════════════════════════════════════════════════╗");
    ConsoleUI.Say("║   20 Questions — Binary Decision Tree Demo       ║");
    ConsoleUI.Say("╠══════════════════════════════════════════════════╣");
    ConsoleUI.Say("║  Think of any object. I will ask yes/no          ║");
    ConsoleUI.Say("║  questions to classify it. When I am wrong I     ║");
    ConsoleUI.Say("║  learn a new question and remember it forever.   ║");
    ConsoleUI.Say("╚══════════════════════════════════════════════════╝");
}
