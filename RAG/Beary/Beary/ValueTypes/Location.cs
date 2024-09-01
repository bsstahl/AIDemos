using ValueOf;

namespace Beary.ValueTypes;

public class Location: ValueOf<Uri,  Location>
{
    public static Location From(string value) => Location.From(new Uri(value));
}
