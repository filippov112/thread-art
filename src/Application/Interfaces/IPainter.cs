using Domain.Models;

namespace Application.Interfaces
{
    public interface IPainter 
    {
        public Task<ImageMatrix> GetPixelMatrixAsync(Stream originalImageStream);
        public Task SaveImageAsync(Stream resultImageStream, int padding, SectorPoint[] points, ImageMatrix image);
    }
}
