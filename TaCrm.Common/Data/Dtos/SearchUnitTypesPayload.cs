namespace TaCrm.Common.Data.Dtos;

public sealed record SearchUnitTypesPayload
{
    public required string[] UnitTypeIds { get; init; }
    public required string BookingPeriodId { get; init; }
    public required DateOnly StartDate { get; init; }
}