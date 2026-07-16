namespace SimilarityScorer.Messages;

internal record SentenceEmbedding(int Index, string Sentence, float[] Embedding);
