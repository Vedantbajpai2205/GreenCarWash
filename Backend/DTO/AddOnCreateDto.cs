using System.ComponentModel.DataAnnotations;

    public class AddOnCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }