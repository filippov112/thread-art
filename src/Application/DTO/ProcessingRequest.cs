namespace Application.DTO;

public class ProcessingRequest
{
    public string FileName { get; set; } = string.Empty;
    public string SystemPath { get; set; } = string.Empty;
    public string WebPath { get; set; } = string.Empty;
    public int CountPoints { get; set; } = 240;
    public int CountSteps { get; set; } = 4000;
    public int ContrastLine { get; set; } = 15;
    public int Padding { get; set; } = 10;
}
