using Beary.Entities;

namespace Cluster.Extensions;

internal static class ChatContentExtensions
{
    internal static void OutputToUser(this ChatContent content, int? selectedSamples = null, int? totalSamples = null)
    {
        var startingColor = Console.ForegroundColor;
        Console.ForegroundColor = content.Role.AsConsoleColor();

        Console.WriteLine();
        if (selectedSamples.HasValue && totalSamples.HasValue)
            Console.Write($"({selectedSamples} / {totalSamples}): ");
        Console.WriteLine(content.Value);
        Console.WriteLine();

        Console.ForegroundColor = startingColor;
    }

    internal static void OutputToUser(this IEnumerable<ChatContent> contents)
    {
        contents.ToList().ForEach(c => c.OutputToUser());
    }

}
