using Domain.Models;

namespace Application.Interfaces
{
    public interface IPainter : IDisposable
    {
        public Task<PixelData[,]> GetPixelMatrixAsync(Stream originalImageStream);
        public Task SaveImageAsync(Stream resultImageStream, int padding, SectorPoint[] points, double[,] values);
    }
}
