using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class OrderAddOn
    {
        [Key]
        public int Id{get; set;}

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public int AddOnId { get; set; }

        [ForeignKey("AddOnId")]
        public AddOn AddOn { get; set; }
    }