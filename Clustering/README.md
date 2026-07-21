# Axiom Clustering Demo

A .NET 10 console application that demonstrates **k-Means clustering** on a set of axioms represented as 768-dimensional embedding vectors. The demo groups semantically similar axioms together and evaluates the quality of the resulting clusters using three standard scoring metrics.

## Overview

Each axiom in `Data/axioms.json` is paired with a pre-computed embedding — a 768-dimensional float vector that captures its semantic meaning. Axioms that are conceptually similar will have embeddings that are close together in that high-dimensional space. k-Means clustering exploits this proximity to automatically group the axioms without any human labelling.

## Running the Demo

From the repository root:

```bash
dotnet run --project Clustering/src/AxiomClusteringDemo -- <k>
```

`<k>` is the number of clusters. It defaults to **4** if omitted.

```bash
# Use the default of 4 clusters
dotnet run --project Clustering/src/AxiomClusteringDemo

# Request 6 clusters
dotnet run --project Clustering/src/AxiomClusteringDemo -- 6
```

An optional second argument overrides the path to the data file:

```bash
dotnet run --project Clustering/src/AxiomClusteringDemo -- 4 /path/to/axioms.json
```

## How k-Means Works

1. **Initialisation** — *k* centroids are seeded by picking *k* distinct axioms at random.
2. **Assignment** — every axiom is assigned to the nearest centroid by Euclidean distance in embedding space.
3. **Update** — each centroid is recomputed as the mean of all axioms currently assigned to it.
4. **Repeat** — steps 2 and 3 iterate until centroids stop moving (within a small tolerance) or a maximum iteration count is reached.

Because of the random seed, results vary between runs. Re-run with the same *k* to observe the natural variation, or try different values of *k* and compare the quality scores to find the best fit.

## Cluster Quality Scores

Three standard metrics are computed and printed at the top of each run. Together they give a rounded picture of how well the clusters are defined.

### Silhouette Score
**Range:** −1 to 1 · **Higher is better**

For each axiom, the score measures how much closer it is to the other members of its own cluster than to the nearest foreign cluster. A score near **1** means the axiom is firmly in the right cluster; near **0** means it sits on a boundary; negative means it probably belongs in a neighbouring cluster. The reported value is the mean across all axioms.

### Davies-Bouldin Index
**Range:** 0 to ∞ · **Lower is better**

For each cluster the index computes the ratio of its internal scatter to its separation from the most similar other cluster, then averages over all clusters. A low value means clusters are compact and well separated from each other.

### Calinski-Harabasz Score
**Range:** 0 to ∞ · **Higher is better**

Also called the Variance Ratio Criterion. It is the ratio of between-cluster dispersion (how spread apart the cluster centres are from the global centre, weighted by cluster size) to within-cluster dispersion (how tightly each cluster is packed). A high value means the clusters are dense and distinct.

## Output Format

```
k-means completed in N iteration(s). Converged: True/False

Cluster Quality Scores
------------------------------------------------------------------------
  Silhouette Score:          0.0312  (higher is better, range: -1..1)
  Davies-Bouldin Index:      3.4120  (lower is better)
  Calinski-Harabasz Score:     2.87  (higher is better)

Cluster 0
------------------------------------------------------------------------
  Representative: <the axiom whose embedding is closest to the centroid>

Cluster 1
------------------------------------------------------------------------
  Representative: <the axiom whose embedding is closest to the centroid>
...
```

The **Representative** for each cluster is the single axiom whose embedding lies closest to that cluster's centroid — a human-readable stand-in for the mathematical centre of the group. The full list of axioms assigned to each cluster can be revealed by uncommenting the loop in `Program.cs`.

## Project Structure

```
Clustering/
├── Data/
│   └── axioms.json                  Input data: axiom text + 768-d embedding
└── src/
    └── AxiomClusteringDemo/
        ├── Program.cs               Entry point: loads data, runs clustering, prints results
        ├── Models/
        │   └── Axiom.cs             DTO for a single axiom (Text + Embedding)
        └── Clustering/
            ├── KMeans.cs            Reusable k-means implementation
            ├── KMeansResult.cs      Result record (Assignments, Centroids, Iterations, Converged)
            ├── VectorMath.cs        Euclidean distance helper
            ├── SilhouetteScore.cs   Silhouette Score calculator
            ├── DaviesBouldinIndex.cs Davies-Bouldin Index calculator
            └── CalinskiHarabaszScore.cs Calinski-Harabasz Score calculator
```

## Dependencies

- [.NET 10](https://dotnet.microsoft.com/) (uses `System.Text.Json` for deserialisation; no third-party packages)
