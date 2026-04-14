namespace Web.Interfaces;

public interface IPathManager
{
    public string OriginalImagePathVM { get; }
    public string ResultImagePathVM { get; }
    public string ResultRouteFilePathVM { get; }

    public string OriginalImagePath { get; }
    public string ResultImagePath { get; }
    public string ResultRouteFilePath { get; }

    public void InitNamesAndPaths(string webRootPath, string originalFileName);
}
