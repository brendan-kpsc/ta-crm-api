namespace TaCrm.Common.Data;

public sealed class BookingPeriodEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsActive { get; init; }
    public required int NumberOfNights { get; init; }
    public required DayOfWeek[] ValidStartDays { get; init; }
    public string Description => 
        "Departs " + 
        string.Join(", ", ValidStartDays[..^1].Select(d => d.ToString("G"))) +
        $" or {ValidStartDays[^1]}";
}