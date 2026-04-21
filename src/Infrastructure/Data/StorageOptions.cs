namespace Infrastructure.Data;

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
    public string FolderPath { get; set; } = string.Empty;
    /// <summary>
    /// Каталог статических файлов
    /// </summary>
    public string StaticFiles { get; set; } = string.Empty;
    /// <summary>
    /// Интервал проверок
    /// </summary>
    public float CleanupIntervalHours { get; set; } = 0.1f;
    /// <summary>
    /// Максимальный возраст жизни файла
    /// </summary>
    public float FileAgeHours { get; set; } = 1f;
}
