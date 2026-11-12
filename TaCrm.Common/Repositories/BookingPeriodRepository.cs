using Dapper;
using MySqlConnector;
using TaCrm.Common.Data;
using TaCrm.Common.Data.Dapper;
using TaCrm.Common.Utils;

namespace TaCrm.Common.Repositories;

public sealed class BookingPeriodRepository (MySqlDataSource dataSource)
{
    private static readonly string[] ValidStartDateColumnNames =
    [
        .. new[] { "sun", "mon", "tue", "wed", "thu", "fri", "sat" }
            .Select(day => $"startdate{day}validyn")
    ];
    
    private static DayOfWeek[] GetValidStartDays(BookingPeriodDapper bookingPeriodDapper)
    {
        return
        [
            .. ValidStartDateColumnNames.Where(colName => 
                    (string) bookingPeriodDapper.GetType().GetProperty(colName)!.GetValue(bookingPeriodDapper)! == "Yes")
                .Select(colName => colName switch
                {
                    "startdatesunvalidyn" => DayOfWeek.Sunday,
                    "startdatemonvalidyn" => DayOfWeek.Monday,
                    "startdatetuevalidyn" => DayOfWeek.Tuesday,
                    "startdatewedvalidyn" => DayOfWeek.Wednesday,
                    "startdatethuvalidyn" => DayOfWeek.Thursday,
                    "startdatefrivalidyn" => DayOfWeek.Friday,
                    "startdatesatvalidyn" => DayOfWeek.Saturday,
                    _ => throw new ArgumentOutOfRangeException(nameof(colName), $"Invalid {nameof(colName)} value: {colName}")
                })
        ];
    }

    private static BookingPeriodEntity MapFromDapper(BookingPeriodDapper bookingPeriodDapper)
    {
        return new BookingPeriodEntity
        {
            Id = Guid.Parse(bookingPeriodDapper.id),
            Name = bookingPeriodDapper.name!,
            IsActive = bookingPeriodDapper.activeyn == "Yes",
            NumberOfNights = bookingPeriodDapper.nightsnum ?? 0,
            ValidStartDays = GetValidStartDays(bookingPeriodDapper)
        };
    }

    public async Task<BookingPeriodEntity> Get(string bookingPeriodId, CancellationToken cancellationToken)
    {
        await using var mySqlConnection = await dataSource.OpenConnectionAsync(cancellationToken);

        var queryString = $"SELECT {MySqlUtils.CreateMySqlDapperColSelect(typeof(BookingPeriodDapper))} " +
                          $"FROM {Constants.MySqlTableNames.BookingPeriod} WHERE id = @Id";
        var queryResult = await mySqlConnection.QuerySingleAsync<BookingPeriodDapper>(queryString, new { Id=bookingPeriodId });
        return MapFromDapper(queryResult);
    }
    
    public async Task<IReadOnlyList<BookingPeriodEntity>> GetAllBookingPeriods(CancellationToken cancellationToken)
    {
        await using var mySqlConnection = await dataSource.OpenConnectionAsync(cancellationToken);
        
        var queryResult = await mySqlConnection.QueryAsync<BookingPeriodDapper>(
            $"SELECT {MySqlUtils.CreateMySqlDapperColSelect(typeof(BookingPeriodDapper))} " +
            $"FROM {Constants.MySqlTableNames.BookingPeriod};");
        
        return queryResult.Select(row => new BookingPeriodEntity
        {
            Id = Guid.Parse(row.id),
            Name = row.name!,
            IsActive = row.activeyn == "Yes",
            NumberOfNights = row.nightsnum ?? 0,
            ValidStartDays = GetValidStartDays(row)
        }).ToList();
    }
}