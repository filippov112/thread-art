using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public interface IPainter: IDisposable
    {
        public Task<double[,]> GetImageGrayNegativeMatrix(string inputImagePath);
        public SizeImage? Size { get; }
        public Task DrawImage(double[,] values, int padding);
        public void DrawCoordinate(SectorPoint point);
        public Task SaveImage(string path);
    }
}
