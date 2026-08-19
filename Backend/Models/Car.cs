using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    public class Car{
        [Key]
        public int Id { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        public int Year { get; set; }

        public bool IsActive { get; set; } = true;
        [Required]
        public string UserId{get;set;}

        [ForeignKey("UserId")]
        public ApplicationUser User {get;set;}
    }