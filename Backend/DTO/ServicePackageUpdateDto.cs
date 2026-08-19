using System.ComponentModel.DataAnnotations;

public class ServicePackageUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public bool IsActive { get; set; }
}