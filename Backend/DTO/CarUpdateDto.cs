using System.ComponentModel.DataAnnotations;
    public class CarUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Make { get; set; }
        
        [Required]
        public string Model { get; set; }
        
        [Required]
        public int Year { get; set; }
        public bool IsActive { get; set; }
    }
