using Domain.Models;

namespace Application.Services
{
    public interface IPainter : IDisposable
    {
        public Task<PixelMatrix> GetPixelMatrix(string inputImagePath);
        public Task<PixelMatrix> DrawImage(RouteMatrix matrix, List<Line> route);
        public Task SaveImage(string path, RouteMatrix matrix, PixelMatrix pixelMatrix);
    }
}
