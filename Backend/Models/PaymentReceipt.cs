using System.ComponentModel.DataAnnotations;

public class PaymentReceipt
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal AmountPaid { get; set; }

    [Required]
    [RegularExpression("^(PAID|NOT_PAID)$", ErrorMessage = "PaymentStatus must be either 'PAID' or 'NOT_PAID'.")]
    public string PaymentStatus { get; set; }
}