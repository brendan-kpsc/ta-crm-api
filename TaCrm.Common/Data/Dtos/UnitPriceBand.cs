using TaCrm.Common.Data.Values;
using TaCrm.Common.Enums;

namespace TaCrm.Common.Data.Dtos;

public sealed record UnitPriceBand
{
    public required string UnitTypeId { get; init; }
    public required string UnitTypeName { get; init; }
    public required string UnitTypeDescription { get; init; }
    public required string UnitTypeOverview { get; init; }
    public required string UnitTypeWebPageUrl { get; init; }
    public required string UnitTypeImageUrl { get; init; }
    public required Season SeasonName { get; init; }
    public required int MaxOccupancy { get; init; }
    public required int AvailableUnitCount { get; init; }
    public required decimal Price { get; init; }
    public required decimal DepositAmount { get; init; }
    public required BookingTimeline Timeline { get; init; }
}