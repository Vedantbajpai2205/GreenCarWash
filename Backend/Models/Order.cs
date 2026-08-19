using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }


        public string? WasherId { get; set; }
        [ForeignKey("WasherId")]
        public ApplicationUser Washer { get; set; }

        [Required]
        public int CarId { get; set; }

        [ForeignKey("CarId")]
        public Car Car { get; set; }

        [Required]
        public int PackageId { get; set; }
        [ForeignKey("PackageId")]
        public ServicePackage Package { get; set; }

        public int? PromoCodeId { get; set; }
        public PromoCode PromoCode { get; set; }

        public DateTime ScheduledDate { get; set; }

        [Required]
        public string Status { get; set; } // PENDING, ACCEPTED, INPROCESS, COMPLETED, CANCELLED

        [Required]
        public string CustomerId{get; set;}
        
        [Required]
        public string Location { get; set; }

        public bool IsCompleted { get; set; }

        public decimal TotalAmount { get; set; }

        public ICollection<OrderAddOn> OrderAddOns { get; set; }

    }