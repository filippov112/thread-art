using Web.Interfaces;

namespace Web.Services;

public class StreamController : IStreamController
{
    public Stream MakeStream(string sourcePath)
    {
        var str = File.Create(sourcePath);
        return str;
    }
    public async Task SaveFromMemory(string sourcePath, MemoryStream memoryStream)
    {
        using var fileStream = File.Create(sourcePath);
        await memoryStream.CopyToAsync(fileStream);
        memoryStream.Position = 0;
    }
}
