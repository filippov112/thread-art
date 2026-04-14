public class FileCleanupService : BackgroundService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileCleanupService> _logger;

    public FileCleanupService(IWebHostEnvironment env, ILogger<FileCleanupService> logger)
    {
        _env = env;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build().GetSection("Storage");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Запуск очистки файлов...");
            CleanupOldFiles(config["FolderPath"] ?? "/storage", float.Parse(config["FileAgeHours"]?.Replace('.', ',') ?? "1"));
            _logger.LogInformation("Очистка завершена.");
            await Task.Delay(TimeSpan.FromHours(double.Parse(config["CleanupIntervalHours"] ?? "1", System.Globalization.CultureInfo.InvariantCulture)), stoppingToken);
        }
    }

    private void CleanupOldFiles(string folderPath, float fileAgeHours)
    {
        string fullPath = Path.Combine(_env.WebRootPath, folderPath);
        if (!Directory.Exists(fullPath))
        {
            _logger.LogWarning($"Папка {fullPath} не существует.");
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
                    _logger.LogInformation($"Удален файл: {file}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при удалении файла {file}: {ex.Message}");
                }
            }
        }
    }
}
