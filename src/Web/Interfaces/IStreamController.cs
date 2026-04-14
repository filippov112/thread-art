namespace Web.Interfaces;

public interface IStreamController
{
    public Stream MakeStream(string sourcePath);
    public Task SaveFromMemory(string sourcePath, MemoryStream memoryStream);
}
