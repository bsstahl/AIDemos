# Semantic Similarity Demo

This demo shows that **semantic search understands intent**, while keyword search only sees words.

A target phrase is compared against four candidates using the **cosine distance** of their embeddings, generated locally via [LM Studio](https://lmstudio.ai). The key result: the phrase that shares *no keywords* with the target ranks as the closest match, while a phrase that shares several exact keywords ranks lower — because it describes the opposite action.

This illustrates why semantic search outperforms keyword search in real-world scenarios such as Q&A retrieval, support ticket routing, and document similarity.

## Prerequisites

1. Install and open **LM Studio**.
2. Download the `mxbai-embed-large-v1` embedding model (the model identifier in LM Studio will be `text-embedding-mxbai-embed-large-v1`).
   > Model choice matters significantly — see the [Model Comparison](#model-comparison) section below.
3. Start the local server: **Local Server → Start Server** (default port `1234`).

## Configuration

All phrases and connection settings are constants at the top of `Program.cs`:

| Constant            | Purpose                                   |
|---------------------|-------------------------------------------|
| `lmStudioUrl`       | LM Studio embeddings endpoint             |
| `modelName`         | Embedding model identifier                |
| `targetPhrase`      | The phrase to compare against             |
| `comparisonPhrases` | The phrases being compared                |

## Running

```bash
dotnet run
```

## What to Expect

Results are sorted by cosine distance (ascending), so the most semantically similar phrase appears first.

The comparison phrases are deliberately chosen so that:
- The **closest match** uses completely different vocabulary but expresses the same intent as the target
- The **second phrase** shares several exact keywords with the target but describes the opposite action
- A keyword search would rank these two in the wrong order; semantic search gets it right

## Model Comparison

The results vary significantly depending on which embedding model is used — and that difference is itself an important part of the story. Not all embedding models are equally good at capturing semantic intent.

| Model | Keyword-trap rank | Semantic match rank | Works as intended? |
|-------|:-----------------:|:-------------------:|:------------------:|
| `text-embedding-mxbai-embed-large-v1` | 2nd (0.203) | **1st (0.193)** | ✅ Yes |
| `text-embedding-nomic-embed-text-v2:2` | **1st (0.174)** | 2nd (0.206) | ❌ No |

The nomic model relies heavily on lexical overlap, so the phrase that shares exact keywords (*laptop*, *battery*, *drain*, *overnight*) with the target outranks the true semantic match. The mxbai model is trained specifically for semantic sentence similarity and correctly surfaces intent over vocabulary.

Try swapping the `modelName` constant to see this difference for yourself.
