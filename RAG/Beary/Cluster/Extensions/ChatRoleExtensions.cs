using Beary.Entities;

namespace Cluster.Extensions;

internal static class ChatRoleExtensions
{
    internal static ConsoleColor AsConsoleColor(this ChatRole role)
        => role switch
        {
            ChatRole.Agent => ConsoleColor.Green,
            ChatRole.User => ConsoleColor.White,
            ChatRole.System => ConsoleColor.Yellow,
            ChatRole.Context => ConsoleColor.Cyan,
            _ => throw new NotImplementedException()
        };
}
