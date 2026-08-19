public class PaymentReceiptResponseDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal AmountPaid { get; set; }
    public string PaymentStatus { get; set; } // Added for status (PAID or NOT_PAID)
    public string CustomerId { get; set; } // For ownership check
}