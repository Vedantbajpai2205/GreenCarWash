public class PromoCodeUpdateDto
{
    public string Code { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTime ValidTill { get; set; }
    public bool IsActive { get; set; }
}