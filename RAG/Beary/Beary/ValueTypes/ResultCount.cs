using ValueOf;

namespace Beary.ValueTypes;

public class ResultCount : ValueOf<int, ResultCount>
{
    const int defaultResultCount = 5;
    const int maxResultCount = 30;

    public static int MaxValue => maxResultCount;

    public ResultCount()
    {
        this.Value = defaultResultCount;
    }

    protected override void Validate()
    {
        if (this.Value < 0)
            throw new ArgumentException($"ResultCount ({this.Value}) must be greater than or equal to 0.");

        if (this.Value > maxResultCount)
            throw new ArgumentException($"ResultCount ({this.Value}) must be less than or equal to {maxResultCount}.");
    }
}
