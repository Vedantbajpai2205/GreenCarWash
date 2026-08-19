using System.ComponentModel.DataAnnotations;
public class RatingResponseDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string UserId { get; set; }
    public string WasherId { get; set; }
    [RegularExpression("^[0-5]$", ErrorMessage = "Rating must be between 0 and 5.")]
    public int RatingValue { get; set; }
}