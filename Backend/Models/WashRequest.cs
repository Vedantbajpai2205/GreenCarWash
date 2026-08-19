using System.ComponentModel.DataAnnotations;

public class WashRequest
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string WasherId { get; set; }
    public ApplicationUser Washer { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }
    public int PackageId { get; set; }
    public ServicePackage Package { get; set; }
    public int? PromoCodeId { get; set; }
    public PromoCode PromoCode { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; }
    public ICollection<OrderAddOn> OrderAddons { get; set; }
}
