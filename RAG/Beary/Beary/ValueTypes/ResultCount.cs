using ValueOf;

namespace Beary.ValueTypes;

public class ResultCount : ValueOf<int, ResultCount>
{
    const int defaultResultCount = 5;
    const int maxResultCount = 10;

    public ResultCount()
    {
        this.Value = defaultResultCount;
    }

    protected override void Validate()
    {
        if (this.Value < 0)
            throw new ArgumentException("ResultCount must be greater than or equal to 0.");

        if (this.Value > maxResultCount)
            throw new ArgumentException($"ResultCount must be less than or equal to {maxResultCount}.");
    }
}
