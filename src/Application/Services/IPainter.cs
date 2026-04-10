using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public interface IPainter: IDisposable
    {
        public Task<double[,]> GetImageGrayNegativeMatrix(string inputImagePath, SizeImage smallSize);
        public Task DrawImage(double[,] values, int padding);
        public void DrawCoordinate(PixelPoint imagePoint, SectorPoint sectorPoint);
        public Task SaveImage(string path);
    }
}
