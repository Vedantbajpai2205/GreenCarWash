using System.ComponentModel.DataAnnotations;

    public class Leaderboard{
        [Key]
        public int Id { get; set; }

        public string WasherId { get; set; }
        public ApplicationUser Washer { get; set; }

        public decimal WaterSaved { get; set; }

        public int Rank { get; set; }
    }