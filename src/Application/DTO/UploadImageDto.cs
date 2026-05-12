namespace Application.DTO;
/// <summary>
/// Возвращаемый результат запроса обработки изображения.
/// </summary>
/// <param name="ResultImagePath">Относительный путь к результирующему изображению.</param>
/// <param name="ResultRoutePath">Относительный путь к текстовому файлу с маршрутом.</param>
public record UploadImageDto(
    string ResultImagePath,
    string ResultRoutePath
);
