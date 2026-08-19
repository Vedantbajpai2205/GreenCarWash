using System.ComponentModel.DataAnnotations;

public class PaymentReceiptCreateDto
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public decimal AmountPaid { get; set; }

    [Required]
    [RegularExpression("^(PAID|NOT_PAID)$", ErrorMessage = "PaymentStatus must be either 'PAID' or 'NOT_PAID'.")]
    public string PaymentStatus { get; set; }
}