namespace SimilarityScorer.Messages;

internal record SimilarityResult(
    List<IndexedSentence> Sentences,
    double[][] CosineSimilarityMatrix,
    List<SimilarityPair> Pairs);
