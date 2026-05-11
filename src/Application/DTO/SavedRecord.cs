namespace Application.DTO;

public record SavedRecord(
    Stream ResultImage,
    Stream RouteFile,
    UploadImageDto Response
) : IDisposable
{
    public void Dispose()
    {
        ResultImage?.Dispose();
        RouteFile?.Dispose();
        GC.SuppressFinalize(this);
    }
}
