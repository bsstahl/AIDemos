namespace Beary.Extensions;

public static class ArgumentNullExceptionExtensions
{
    public static void ThrowIfNull<T>(this T value, string argumentName)
    {
        if (value is null)
            throw new ArgumentNullException(argumentName);
    }
}
