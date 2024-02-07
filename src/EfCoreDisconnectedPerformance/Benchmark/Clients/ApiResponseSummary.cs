namespace Benchmark.Clients;

public record ApiResponseSummary(int NumberOfOkCallsOnGet, int NumberOfOkCallsOnUpdate, int NumberOfNotFoundCallsOnGet, int NumberOfNotFoundCallsOnUpdate, int NumberOfConflictCallsOnUpdate)
{
    public ApiResponseSummary()
        : this(0, 0, 0, 0, 0)
    {
    }

    public static ApiResponseSummary operator +(ApiResponseSummary a, ApiResponseSummary b)
    {
        return new(
            a.NumberOfOkCallsOnGet + b.NumberOfOkCallsOnGet,
            a.NumberOfOkCallsOnUpdate + b.NumberOfOkCallsOnUpdate,
            a.NumberOfNotFoundCallsOnGet + b.NumberOfNotFoundCallsOnGet,
            a.NumberOfNotFoundCallsOnUpdate + b.NumberOfNotFoundCallsOnUpdate,
            a.NumberOfConflictCallsOnUpdate + b.NumberOfConflictCallsOnUpdate
            );
    }
}
