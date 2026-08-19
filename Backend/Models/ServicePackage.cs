using System.ComponentModel.DataAnnotations;
    public class ServicePackage{
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
