namespace SimilarityScorer.Messages;

internal record SimilarityPair(
    int SentenceAIndex,
    string SentenceA,
    int SentenceBIndex,
    string SentenceB,
    double CosineSimilarity);
