using Domain.ImageProcessor.Models;

namespace Application.ImageProcessor.Services;

/// <summary>
/// Сервис работы с файловой системой
/// </summary>
public interface IFileSystemService
{
    public Task<ImageMatrix> ReadOriginalImageAsync(string systemPath, int countSidePoints, CancellationToken ct = default);
    public Task<string> SaveOriginalImageAsync(string fileName, Stream originalStream, CancellationToken cancellationToken = default);
    public Task<string> SaveRouteAsync(Route route, CancellationToken cancellationToken = default);
    public Task<string> SaveResultImageAsync(int innerWidth, int innerHeight, int padding, int[] data, SectorPoint[] sidePoints, CancellationToken ct = default);

}
