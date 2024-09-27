using ValueOf;

namespace Beary.ValueTypes;

public class Vector: ValueOf<IEnumerable<Single>, Vector>
{
    public Single[] ToArray() => Value.ToArray();

    public Double[] ToDoubleArray() => Value.Select(s => (Double)s).ToArray();
}
