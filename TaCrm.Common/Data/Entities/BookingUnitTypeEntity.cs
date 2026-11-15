namespace TaCrm.Common.Data;

public sealed class BookingUnitTypeEntity
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Overview { get; init; }
    public required string ImageUrl { get; init; }
    public required string WebPageUrl { get; init; }
    public required int MaxOccupancy { get; init; }
}