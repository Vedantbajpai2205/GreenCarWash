using System.ComponentModel.DataAnnotations;

public class ServicePackageCreateDto
{
    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;
}