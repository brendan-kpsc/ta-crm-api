using TaCrm.Common.Data;
using TaCrm.Common.Data.Dtos;
using TaCrm.Common.Data.Values;
using TaCrm.Common.Repositories;

namespace TaCrm.Common.Handlers;

public sealed class BookingUnitTypeHandler(
    BookingUnitTypeRepository bookingUnitTypeRepository,
    BookingPeriodRepository bookingPeriodRepository)
{
    public async Task<IReadOnlyList<BookingUnitTypeEntity>> GetBookableUnitTypes(CancellationToken cancellationToken) =>
        await bookingUnitTypeRepository.GetBookableUnitTypes(cancellationToken);

    public async Task<IReadOnlyList<UnitPriceBand>> SearchAvailableUnitTypes(SearchUnitTypesPayload payload, CancellationToken cancellationToken)
    {
        var bookingPeriod = await bookingPeriodRepository.Get(payload.BookingPeriodId, cancellationToken);
        var bookingTimeline = new BookingTimeline
        {
            StartDate = payload.StartDate,
            NumNights = bookingPeriod.NumberOfNights
        };

        var availablePriceBands = await Task.WhenAll(payload.UnitTypeIds.Select(id => 
            bookingUnitTypeRepository.GetAvailablePriceBands(id, bookingTimeline, cancellationToken))
        );

        return availablePriceBands.SelectMany(list => list).ToList();
    }
}