namespace Infrastructure.ImageProcessor.Settings;

/// <summary>
/// Конфигурация хранилища
/// </summary>
public class StorageOptions
{
    /// <summary>
    /// Секция файла конфигурации
    /// </summary>
    public const string SectionName = "Storage";
    /// <summary>
    /// Подкаталог для хранения результатов (копия загруженного файла, обработанное изображение, файл с маршрутом)
    /// </summary>
    public string FolderPath { get; set; } = "C:\\D\\thread-art";
}
