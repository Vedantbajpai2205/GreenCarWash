public class OrderResponseDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string WasherId { get; set; }
    public int CarId { get; set; }
    public int PackageId { get; set; }
    public int? PromoCodeId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; }
    public string Location { get; set; }
    public bool IsCompleted { get; set; }
    public decimal TotalAmount { get; set; }
    public List<int> AddOnIds { get; set; }
}