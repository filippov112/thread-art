namespace Application.DTO;

/// <summary>
/// DTO для возврата информации о сохранённых файлах.
/// </summary>
public record ProcessingResponse(
    string OriginalImage, // Например, "/storage/file123.jpg"
    string ResultImage,   // Например, "/storage/result456.png"
    string ResultRoute    // Например, "/storage/route789.txt"
);
