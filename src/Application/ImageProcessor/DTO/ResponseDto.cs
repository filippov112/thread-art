namespace Application.ImageProcessor.DTO;

/// <summary>
/// Результат обработки изображения
/// </summary>
public record ResponseDto(
    string ResultImagePath,
    string ResultRoutePath
);
