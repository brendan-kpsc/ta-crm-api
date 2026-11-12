namespace TaCrm.Common.Data.Values;

public sealed record BookingTimeline
{
    public required DateOnly StartDate { get; init; }
    public required int NumNights { get; init; }
    
    public DateOnly EndDate =>
        StartDate.AddDays(NumNights);
};