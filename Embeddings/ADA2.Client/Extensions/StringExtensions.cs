namespace ADA2.Client.Extensions;

public static class StringExtensions
{
    public static (bool IsValid, Uri? Uri) TryCreateUri(this string Value)
    {
        return (Uri.TryCreate(Value, UriKind.Absolute, out Uri? result), result);
    }

    public static Uri? AsUri(this string value) => TryCreateUri(value).Uri;
}
