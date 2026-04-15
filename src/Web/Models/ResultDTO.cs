namespace Web.Models;

public class ResultDTO
{
    /// <summary>
    /// Относительный путь к исходному изображению.
    /// </summary>
    public string OriginalImagePath { get; set; } = string.Empty;
    /// <summary>
    /// Относительный путь к результирующему изображению.
    /// </summary>
    public string ResultImagePath { get; set; } = string.Empty;
    /// <summary>
    /// Относительный путь к текстовому файлу с маршрутом.
    /// </summary>
    public string ResultRoutePath { get; set; } = string.Empty;
}
