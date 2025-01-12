namespace GD.Extensions;

internal static class ArgumentExtensions
{
    internal static string ParameterPath(this string[] args)
    {
        return (args.Any() && !string.IsNullOrWhiteSpace(args[0]))
            ? args[0]
            : string.Empty;
    }

    internal static bool ContinueTraining(this string[] args)
    {
        return (args.Any() && args.Length > 1 && bool.TryParse(args[1], out var result)) 
            ? result 
            : false;
    }
}
