namespace Web.Models;

/// <summary>
/// JWT конфигурация
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Секция файла конфигурации
    /// </summary>
    public const string SectionName = "JwtSettings";
    /// <summary>
    /// Ключ
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    /// <summary>
    /// Отправитель
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    /// <summary>
    /// Обработчик
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    /// <summary>
    /// Время жизни токена
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;
}
