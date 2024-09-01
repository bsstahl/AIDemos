using ValueOf;
using Beary.Extensions;

namespace Beary.ValueTypes;

public class Identifier: ValueOf<String, Identifier>
{
    public static Identifier From(Guid guidValue) => Identifier.From(guidValue.ToString());
    public static Identifier From(Uri uriValue)
    {
        uriValue.ThrowIfNull(nameof(uriValue));
        return Identifier.From(uriValue.ToString());
    }
}
