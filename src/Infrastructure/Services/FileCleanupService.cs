using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class FileCleanupService(ILogger<FileCleanupService> logger, IConfigurationBuilder builder) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build().GetSection("Storage");
        var path = Path.Combine(config["StaticFiles"] ?? "wwwroot", config["FolderPath"] ?? "storage");
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Запуск очистки файлов...");
            CleanupOldFiles(path, float.Parse(config["FileAgeHours"]?.Replace('.', ',') ?? "1"));
            logger.LogInformation("Очистка завершена.");
            await Task.Delay(TimeSpan.FromHours(double.Parse(config["CleanupIntervalHours"] ?? "1", System.Globalization.CultureInfo.InvariantCulture)), stoppingToken);
        }
    }

    private void CleanupOldFiles(string folderPath, float fileAgeHours)
    {
        string fullPath = Path.Combine(folderPath);
        if (!Directory.Exists(fullPath))
        {
            logger.LogWarning($"Папка {fullPath} не существует.");
            return;
        }

        var files = Directory.GetFiles(fullPath);
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.LastWriteTime < DateTime.Now.AddHours(-fileAgeHours))
            {
                try
                {
                    fileInfo.Delete();
                    logger.LogInformation($"Удален файл: {file}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Ошибка при удалении файла {file}: {ex.Message}");
                }
            }
        }
    }
}
