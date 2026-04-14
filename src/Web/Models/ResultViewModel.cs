public class ResultViewModel
{
    public IFormFile? ImageFile { get; set; }
    public string OriginalImagePath { get; set; } = string.Empty;
    public string ResultImagePath { get; set; } = string.Empty;
    public string ResultRoutePath { get; set; } = string.Empty;
    public int CountPoints { get; set; } = 240;
    public int CountSteps { get; set; } = 4000;
    public double ContrastLine { get; set; } = 1;
}
