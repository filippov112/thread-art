using Domain.Models;

namespace Application.Interfaces;

public interface IFileSystemService
{
    public Task<ImageMatrix> ReadOriginalImageAsync(string systemPath, CancellationToken ct = default);
    public Task<(string, string)> SaveOriginalImageAsync(string fileName, Stream originalStream, CancellationToken cancellationToken = default);
    public Task<(string, string)> SaveRouteAsync(Route route, CancellationToken cancellationToken = default);
    public Task<(string, string)> SaveResultImageAsync(int padding, SectorPoint[] points, ImageMatrix matrix, CancellationToken ct = default);

}
