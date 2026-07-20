# LinearClassification — 20 Questions Demo

A C# console application that plays a "20 Questions"-style guessing game to
demonstrate **linear classification** via a **binary decision tree**.  
Built with **.NET 10** and **C# 14**.

---

## Contents

- [Concept: Linear Classification and Decision Trees](#concept-linear-classification-and-decision-trees)
- [How the Demo Works](#how-the-demo-works)
- [What to Review](#what-to-review-in-the-source-code)
- [Project Structure](#project-structure)
- [Running the Application](#running-the-application)
- [Example Session](#example-session)
- [Knowledge Persistence](#knowledge-persistence)

---

## Concept: Linear Classification and Decision Trees

### What Is a Classifier?

A **classifier** is a function that maps an input (an object to be identified)
to one of a set of known categories (labels).  Every time the program makes a
guess — "Is it a cat?" — it is performing a classification.

### What Makes a Classifier "Linear"?

A **linear classifier** makes each individual decision by evaluating a single
binary condition — one that divides the known world cleanly into two groups.
This is analogous to drawing a straight line (or hyperplane) through a feature
space that separates two classes.

A **binary decision tree** is the natural, human-readable implementation of
this idea:

- Each **internal node** asks one binary (yes/no) question — a single linear
  split on one feature.
- Each **leaf node** is a classification result — the program's guess.
- The **path from root to leaf** is a sequence of such linear splits, each one
  narrowing the candidate set until only one class remains.

Because every branching decision is binary, the tree partitions the feature
space into non-overlapping regions using a series of axis-aligned linear
boundaries.  The tree as a whole therefore produces a **piecewise-linear
decision boundary** — the hallmark of this family of classifiers.

### Why This Is a Useful Demonstration

| Property | This Demo |
|---|---|
| Feature evaluation | Each yes/no question tests one feature of the object |
| Binary split | Every question divides the remaining candidates into exactly two groups |
| Classification | The leaf reached at the end of traversal is the predicted class |
| Online learning | When wrong, the tree grows by inserting a new question node at the point of failure |
| Persistence | The learned model is saved as JSON and reloaded on the next run |

The game makes the mechanics of a binary decision tree tangible: you can watch
the tree grow in `knowledge.json` after each round and directly see how each
new question reshapes the decision boundary.

---

## How the Demo Works

1. **Startup** — the program loads the decision tree from `knowledge.json`.
   If the file does not exist it initialises a minimal two-leaf tree:

   ```
   Is it an animal?
   ├── yes → cat
   └── no  → rock
   ```

2. **Traversal** — the program walks the tree from root to leaf, asking the
   question at each internal node and following the yes or no branch based on
   the user's answer.

3. **Guess** — when a leaf is reached the program states its guess.

4. **Correct guess** — the round ends; the tree is unchanged.

5. **Wrong guess (learning)** — the program:
   - Asks what object the user was thinking of.
   - Asks for a yes/no question that distinguishes the new object from the
     incorrect guess.
   - Asks which answer applies to the new object.
   - **Replaces the incorrect leaf with a new question node**, attaching the
     old guess and the new object as its two children on the appropriate
     branches.
   - Saves the updated tree to `knowledge.json`.

6. **Loop** — the user can play as many rounds as they like.  The tree grows
   richer with each mistake, improving classification accuracy over time.

---

## What to Review in the Source Code

Readers interested in linear classification and decision trees should focus on
these files in this order:

### 1. `Node.cs` — The Data Structure

This is the core of the classifier.  A single `Node` class represents both
internal question nodes and leaf guess nodes through nullable properties.

Key things to notice:
- The **dual-purpose design** (one class, two roles) mirrors how decision tree
  implementations typically store the tree.
- The `IsLeaf` computed property cleanly expresses the structural invariant.
- The C# 14 **`field` keyword** (semi-auto properties) trims user input on
  assignment, demonstrating how modern language features reduce boilerplate.

### 2. `GameEngine.cs` — Traversal and the Learning Algorithm

This is where the classifier runs and learns.

**`PlayRound()`** — tree traversal (inference):
```
current = root
while current is not a leaf:
    answer = ask(current.Question)
    current = answer ? current.Yes : current.No
return current.Guess          // ← the classification result
```
Each iteration is one linear feature evaluation.  The sequence of evaluations
from root to leaf is the classification path.

**`LearnFromMistake()`** — online tree expansion (training):

The ASCII diagram in the XML doc comment illustrates the structural
transformation.  The critical insight is:

> The incorrect leaf is not deleted — it becomes one child of the new question
> node.  The tree grows at the exact point where the current model failed,
> adding precisely the information needed to distinguish the two objects.

The `parent` / `parentTookYes` variables tracked during traversal are the
mechanism that makes this local, surgical update possible without re-building
the entire tree.

### 3. `TreeRepository.cs` — Model Persistence

Shows how the in-memory tree (the trained model) is serialised to JSON and
reloaded.  The `CreateSeedTree()` method defines the **prior** — the initial
knowledge the classifier starts with before any training.

### 4. `knowledge.json` (generated at runtime)

After a few rounds, open this file and read the tree structure directly.  Each
object in the JSON is a node; nested `Yes`/`No` objects are its children.  You
can see the decision boundary literally written out as a nested data structure.

---

## Project Structure

```
LinearClassification/
├── LinearClassification.csproj   # .NET 10, C# 14, all analysers enabled
├── Program.cs                    # Entry point and game loop
├── Node.cs                       # Binary tree node (data model)
├── ConsoleUI.cs                  # Console I/O helpers (yes/no parsing)
├── TreeRepository.cs             # JSON load/save (model persistence)
├── GameEngine.cs                 # Traversal + learning algorithm
└── knowledge.json                # Generated at runtime — the trained model
```

---

## Running the Application

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
cd LinearClassification
dotnet run
```

`knowledge.json` will be created in the working directory on the first learning
event and updated automatically after that.

To reset the classifier back to its initial state, delete `knowledge.json`:

```bash
# Windows
del knowledge.json

# macOS / Linux
rm knowledge.json
```

---

## Example Session

```
╔══════════════════════════════════════════════════╗
║   20 Questions — Binary Decision Tree Demo       ║
╠══════════════════════════════════════════════════╣
║  Think of any object. I will ask yes/no          ║
║  questions to classify it. When I am wrong I     ║
║  learn a new question and remember it forever.   ║
╚══════════════════════════════════════════════════╝

Think of any object — I'll try to guess it by asking yes/no questions.

Is it an animal? (yes/no): yes

My guess: "cat"
Was I right? (yes/no): no

I give up! What were you thinking of?: dog
Give me a yes/no question that distinguishes "dog" from "cat": Does it bark?
For "dog", is the answer to "Does it bark?" yes? (yes/no): yes
✓ Knowledge base updated — I won't make that mistake again!
```

After this round `knowledge.json` contains:

```json
{
  "Question": "Is it an animal?",
  "Yes": {
    "Question": "Does it bark?",
    "Yes": { "Guess": "dog" },
    "No":  { "Guess": "cat" }
  },
  "No": { "Guess": "rock" }
}
```

The tree now has **three leaves** and can correctly classify a dog, a cat, and
a rock.  Each additional round of learning adds at most one new question node
and one new leaf.
