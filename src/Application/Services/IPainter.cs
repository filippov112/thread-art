using Domain.Models;

namespace Application.Services
{
    public interface IPainter : IDisposable
    {
        public Task<PixelMatrix> GetPixelMatrix(string inputImagePath);
        public SizeImage? Size { get; }
        public Task DrawImage(RouteMatrix matrix, List<Line> route);
        public void DrawCoordinateGrid(RouteMatrix matrix);
        public Task SaveImage(string path);
    }
}
