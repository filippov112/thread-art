using Web.Interfaces;

namespace Web.Services;

public class PathManager : IPathManager
{
    public string OriginalImagePathVM { get; private set; } = string.Empty;

    public string ResultImagePathVM { get; private set; } = string.Empty;

    public string ResultRouteFilePathVM { get; private set; } = string.Empty;

    public string OriginalImagePath { get; private set; } = string.Empty;

    public string ResultImagePath { get; private set; } = string.Empty;

    public string ResultRouteFilePath { get; private set; } = string.Empty;

    private readonly string _storagePath = "storage";

    public PathManager()
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build().GetSection("Storage");
        if (config["FolderPath"] != null)
            _storagePath = config["FolderPath"]!;
    }

    public void InitNamesAndPaths(string webRootPath, string originalFileName)
    {
        string originalImageName = Guid.NewGuid().ToString() + Path.GetExtension(originalFileName);
        string resultImageName = Guid.NewGuid().ToString() + ".png";
        string resultRouteFileName = Guid.NewGuid().ToString() + ".txt";

        OriginalImagePathVM = '/' + Path.Combine(_storagePath, originalImageName);
        ResultImagePathVM = '/' + Path.Combine(_storagePath, resultImageName);
        ResultRouteFilePathVM = '/' + Path.Combine(_storagePath, resultRouteFileName);

        OriginalImagePath = Path.Combine(webRootPath, _storagePath, originalImageName);
        ResultImagePath = Path.Combine(webRootPath, _storagePath, resultImageName);
        ResultRouteFilePath = Path.Combine(webRootPath, _storagePath, resultRouteFileName);

        Directory.CreateDirectory(Path.Combine(webRootPath, _storagePath));
    }
}

