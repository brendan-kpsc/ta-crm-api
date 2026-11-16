using Dapper;
using MySqlConnector;
using TaCrm.Common.Data;
using TaCrm.Common.Data.Dapper;
using TaCrm.Common.Data.Dtos;
using TaCrm.Common.Data.Values;
using TaCrm.Common.Enums;
using TaCrm.Common.Utils;

namespace TaCrm.Common.Repositories;

public sealed class BookingUnitTypeRepository(MySqlDataSource database)
{
    private static BookingUnitTypeEntity MapFromDapper(BookingUnitTypeDapper bookingUnitTypeDapper) =>
        new()
        {
            Id = bookingUnitTypeDapper.id,
            Name = bookingUnitTypeDapper.name,
            Description = bookingUnitTypeDapper.unit_description,
            Overview = bookingUnitTypeDapper.unit_overview,
            ImageUrl = bookingUnitTypeDapper.unit_image_url,
            WebPageUrl = bookingUnitTypeDapper.unit_web_page ?? "https://twinanchors.com/houseboats/",
            MaxOccupancy = bookingUnitTypeDapper.max_occupancy
        };
    
    private async Task<string> GetBookingViewQuery(CancellationToken cancellationToken)
    {
        await using var mySqlConnection = await database.OpenConnectionAsync(cancellationToken);

        var bookingViewCommand = new MySqlCommand(
            "SELECT get_web_booking_view() as QUERY;",
            mySqlConnection
        );
        
        var bookingViewQuery = await bookingViewCommand.ExecuteScalarAsync(cancellationToken);
            
        return bookingViewQuery!.ToString()!;
    }

    public async Task<BookingUnitTypeEntity> Get(string id, CancellationToken cancellationToken)
    {
        await using var mySqlConnection = await database.OpenConnectionAsync(cancellationToken);
        var queryString = $"SELECT {MySqlUtils.CreateMySqlDapperColSelect(typeof(BookingUnitTypeDapper))} " +
                          $"FROM {Constants.MySqlTableNames.BookingUnitType} WHERE id = @id";
        var queryResult = await mySqlConnection.QuerySingleAsync<BookingUnitTypeDapper>(queryString, new {id});

        return MapFromDapper(queryResult);
    }
    
    public async Task<IReadOnlyList<BookingUnitTypeEntity>> GetBookableUnitTypes(CancellationToken cancellationToken)
    {
        await using var connection = await database.OpenConnectionAsync(cancellationToken);
        
        var queryString = $"SELECT {MySqlUtils.CreateMySqlDapperColSelect(typeof(BookingUnitTypeDapper))} " +
                          $"FROM {Constants.MySqlTableNames.BookingUnitType} " +
                          $"WHERE deleted = false AND name NOT IN ('CO', 'MO', 'NB', 'Change Over Unit', 'Non-Boat Unit', 'Non Legacy 94') " +
                          $"AND unit_description NOT IN ('CO', 'MO', 'NB', 'Change Over Unit', 'Non-Boat Unit', 'Non Legacy 94') " +
                          $"AND name <> '' AND unit_description <> '' ORDER BY unit_description;";
        var queryResult = await connection.QueryAsync<BookingUnitTypeDapper>(queryString);

        return queryResult.Select(MapFromDapper).ToList();
    }

    public async Task<IReadOnlyList<UnitPriceBand>> GetAvailablePriceBands(
        string unitTypeId, BookingTimeline bookingTimeline, CancellationToken cancellationToken)
    {
        var availablePriceBands = new List<UnitPriceBand>();
        var bookingViewQuery = await GetBookingViewQuery(cancellationToken);

        await using var mySqlConnection = await database.OpenConnectionAsync(cancellationToken);

        var priceBandsQueryCommand = CreatePriceBandsQueryCommand(
            bookingViewQuery, unitTypeId, bookingTimeline, mySqlConnection);
        
        var queryResult = await priceBandsQueryCommand.ExecuteReaderAsync(cancellationToken);

        while (await queryResult.ReadAsync(cancellationToken))
        {
            var priceBandUnitTypeId = queryResult.GetString("unit_type_id");
            var unitTypeName = queryResult.GetString("unit_type");
            var unitImageUrl = queryResult.GetString("unit_image_url");
            var seasonNameString = queryResult.GetString("season_date_band_name");
            var seasonName = seasonNameString switch
            {
                "Low Season" => Season.Low,
                "Mid Season" => Season.Mid,
                "Peak Season" => Season.Peak,
                _ => throw new ArgumentOutOfRangeException(nameof(Season), $"Value: {seasonNameString}")
            };
            var maxOccupancy = queryResult.GetInt32($"max_occupancy");
            var availableUnitCount = queryResult.GetInt32($"num_{bookingTimeline.NumNights}_day_avail");
            var price = queryResult.GetDecimal($"rate_{bookingTimeline.NumNights}_day");
            var depositAmount = ((string[])["CCV", "CCVI"]).Contains(unitTypeName) ? 1500 : 500;
            var startDate = queryResult.GetDateOnly("date_slot_date");

            var unitTypeEntity = await Get(priceBandUnitTypeId, cancellationToken);

            availablePriceBands.Add(new UnitPriceBand
            {
                UnitTypeId = priceBandUnitTypeId,
                UnitTypeName = unitTypeName,
                UnitTypeOverview = unitTypeEntity.Overview,
                UnitTypeDescription = unitTypeEntity.Description,
                UnitTypeWebPageUrl = unitTypeEntity.WebPageUrl,
                UnitTypeImageUrl = unitImageUrl,
                MaxOccupancy = maxOccupancy,
                SeasonName = seasonName,
                DepositAmount = depositAmount,
                Price = price,
                AvailableUnitCount = availableUnitCount,
                Timeline = new BookingTimeline{StartDate = startDate, NumNights = bookingTimeline.NumNights}
            });
        }

        return availablePriceBands;
        
        MySqlCommand CreatePriceBandsQueryCommand(string bookingViewQuery, string unitTypeId, BookingTimeline timeline,
            MySqlConnection mySqlConnection)
        {
            var priceBandsSql = $"{bookingViewQuery} AND bkng_bookingunitslot.date_slot_date = @startDate " +
                                $"AND get_num_avail_by_group_type(bkng_bookingunittype.id, " +
                                $"(case bkng_bookingunit.unit_grouping when 'TH_SU' then 'TH/SU' when 'SAT_TUE' then 'SAT/TUE' when 'FR_MO' then 'FR/MO' else 'NO GRP' end), bkng_bookingunitslot.date_slot_date, " +
                                $"@dateCount) > 0 AND (bkng_bookingunittype.id = @unitTypeId) ORDER BY unit_type, unit_grouping;";
            var priceBandsCommand = new MySqlCommand(priceBandsSql, mySqlConnection);
            
            priceBandsCommand.Parameters.AddWithValue("@startDate", timeline.StartDate.ToString("yyyy-MM-dd"));
            priceBandsCommand.Parameters.AddWithValue("@dateCount", timeline.NumNights);
            priceBandsCommand.Parameters.AddWithValue("@unitTypeId", $"{unitTypeId}");
            
            return priceBandsCommand;
        }
    }
}