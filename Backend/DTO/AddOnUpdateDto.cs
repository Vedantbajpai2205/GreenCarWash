using System.ComponentModel.DataAnnotations;

    public class AddOnUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }