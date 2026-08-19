public class PromoCodeCreateDto
{
    public string Code { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTime ValidTill { get; set; }
}