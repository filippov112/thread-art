namespace Domain.Models;

public class ImageModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string OriginalFilePath { get; set; }
    public required string ResultImagePath { get; set; }
    public required string ResultRoutePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
