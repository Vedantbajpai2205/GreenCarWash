public class OrderCreateDto
{
    public int CarId { get; set; }
    public int PackageId { get; set; }
    public int? PromoCodeId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Location { get; set; }
    public List<int> AddOnIds { get; set; } = new List<int>();
}