using Infrastructure.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class FileCleanupService(ILogger<FileCleanupService> logger, IOptions<StorageOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var path = Path.Combine(options.Value.StaticFiles, options.Value.FolderPath);
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Запуск очистки файлов...");
            CleanupOldFiles(path, options.Value.FileAgeHours);
            logger.LogInformation("Очистка завершена.");
            await Task.Delay(TimeSpan.FromHours(options.Value.CleanupIntervalHours), stoppingToken);
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
