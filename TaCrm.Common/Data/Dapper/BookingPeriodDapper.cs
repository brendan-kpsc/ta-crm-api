namespace TaCrm.Common.Data.Dapper;

public sealed class BookingPeriodDapper
{
    public string id { get; set; } = null!;
    public string? name { get; set; }
    public DateTime? date_entered { get; set; }
    public DateTime? date_modified { get; set; }
    public string? modified_user_id { get; set; }
    public string? created_by { get; set; }
    public string? description { get; set; }
    public bool? deleted { get; set; }
    public string? assigned_user_id { get; set; }
    public int? nightsnum { get; set; }
    public int? sortorder { get; set; }
    public string? activeyn { get; set; }
    public string? startdatesunvalidyn { get; set; }
    public string? startdatemonvalidyn { get; set; }
    public string? startdatetuevalidyn { get; set; }
    public string? startdatewedvalidyn { get; set; }
    public string? startdatethuvalidyn { get; set; }
    public string? startdatefrivalidyn { get; set; }
    public string? startdatesatvalidyn { get; set; }
}
