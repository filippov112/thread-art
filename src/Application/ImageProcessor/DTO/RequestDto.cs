namespace Application.ImageProcessor.DTO;

/// <summary>
/// Запрос на обработку изображения
/// </summary>
public class RequestDto
{
    public Guid JobID { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string SystemPath { get; set; } = string.Empty;
    public int CountPoints { get; set; } = 240;
    public int CountSteps { get; set; } = 4000;
    public int Padding { get; set; } = 10;
}
