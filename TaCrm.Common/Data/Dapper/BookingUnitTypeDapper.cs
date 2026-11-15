namespace TaCrm.Common.Data.Dapper;

public sealed class BookingUnitTypeDapper
{
    public string id { get; set; }
    public string name { get; set; }
    public string unit_description { get; set; }
    public string unit_overview { get; set; }
    public string unit_image_url { get; set; }
    public string? unit_web_page { get; set; }
    public int max_occupancy { get; set; }
}