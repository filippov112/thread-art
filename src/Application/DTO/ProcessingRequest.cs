namespace Application.DTO;

public class ProcessingRequest
{
    public Stream OriginalImageStream { get; set; } = null!;
    public Stream ResultImageStream { get; set; } = null!;
    public Stream ResultRouteStream { get; set; } = null!;
    public int CountPoints { get; set; } = 240;
    public int CountSteps { get; set; } = 4000;
    public double ContrastLine { get; set; } = 15;
    public int Padding { get; set; } = 10;
}
