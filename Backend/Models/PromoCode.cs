using System.ComponentModel.DataAnnotations;
    public class PromoCode{
        [Key]
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        public decimal DiscountPercent { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime ValidTill { get; set; }
    }