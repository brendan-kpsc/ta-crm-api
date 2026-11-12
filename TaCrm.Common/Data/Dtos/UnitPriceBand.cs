using TaCrm.Common.Data.Values;

namespace TaCrm.Common.Data.Dtos;

public sealed record UnitPriceBand
{
    public required string UnitTypeId { get; init; }
    public required string UnitTypeName { get; init; }
    public required string SeasonName { get; init; }
    public required int AvailableUnitCount { get; init; }
    public required decimal Price { get; init; }
    public required decimal DepositAmount { get; init; }
    public required BookingTimeline Timeline { get; init; }
}