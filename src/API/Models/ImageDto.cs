using Domain.Models;

public class ImageDto
{
    public IFormFile? ImageFile { get; set; }
    public string WebRootPath { get; set; } = string.Empty;
    public string OriginalImagePath { get; set; } = string.Empty;
    public string ResultImagePath { get; set; } = string.Empty;
    public string ResultRoutePath { get; set; } = string.Empty;

    public SizeImage SmallSize { get; set; } = new(270, 270);
    public SizeImage LargeSize { get; set; } = new(540, 540);
    public int CountPoints { get; set; } = 240;
    public int CountSteps { get; set; } = 4000;
    public bool IsEllipse { get; set; } = false;
    public double ContrastLine { get; set; } = 0.1;
}
