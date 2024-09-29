using Beary.Documents.Interfaces;

namespace Beary.Data.Test.Extensions;

public static class EmbeddingsReadRepoExtensions
{
    public async static Task<long> WaitForDocumentIndexing(this IReadEmbeddingsSearchDocuments readRepo, long startingDocCount)
    {
        int delayCount = 0;
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        do { delayCount++; } while (await readRepo.GetDocumentCount() <= startingDocCount);
        stopwatch.Stop();
        return delayCount; // stopwatch.ElapsedMilliseconds;
    }
}
