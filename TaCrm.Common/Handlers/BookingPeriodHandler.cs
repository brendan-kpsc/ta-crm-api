using TaCrm.Common.Data;
using TaCrm.Common.Repositories;

namespace TaCrm.Common.Handlers;

public sealed class BookingPeriodHandler (BookingPeriodRepository bookingPeriodRepository)
{
    public async Task<IReadOnlyList<BookingPeriodEntity>> GetAllBookingPeriods(CancellationToken cancellationToken) =>
        await bookingPeriodRepository.GetAllBookingPeriods(cancellationToken);
}