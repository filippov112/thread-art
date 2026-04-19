using Application.Interfaces;
using Domain.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services
{
    public class Painter : IPainter
    {
        #region Settings
        // Цвета для разных секторов/сторон
        private readonly Dictionary<char, Color> _colors = new() {
                {'A', Color.Red},
                {'B', Color.Blue},
                {'C', Color.Green},
                {'D', Color.Purple},
                {'T', Color.Red},
                {'R', Color.Green},
                {'L', Color.Purple}
            };

        //private readonly Font _font = SystemFonts.CreateFont("Arial", 8);
        #endregion

        #region API
        public async Task<PixelData[,]> GetPixelMatrixAsync(Stream originalImageStream)
        {
            using var image = await Image.LoadAsync<Rgba32>(originalImageStream);
            var matrix = new PixelData[image.Width, image.Height];
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                    matrix[x, y] = new(image[x, y].R, image[x, y].G, image[x, y].B);
            return matrix;
        }

        public async Task SaveImageAsync(Stream resultImageStream, int padding, SectorPoint[] points, double[,] values)
        {
            using var image = DrawCoordinateGrid(padding, points, values);
            image.Save(resultImageStream, new PngEncoder());
        }
        #endregion

        #region Tools
        private Image<Rgba32> RestoreImage(double[,] values)
        {
            Image<Rgba32> image = new(values.GetLength(0), values.GetLength(1), new Rgba32(255, 255, 255, 255));

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    var value = (byte)values[x, y];
                    image[x, y] = new(value, value, value);
                }
            return image;
        }

        private Image<Rgba32> DrawCoordinateGrid(int padding, SectorPoint[] points, double[,] values)
        {
            var image = RestoreImage(values);
            foreach (SectorPoint sectorPoint in points)
            {
                var color = _colors.TryGetValue(sectorPoint.Sector, out Color value) ? value : Color.Black;
                var markerBrush = new SolidBrush(color);

                // Рисуем маркер точки (круг) - исправленная версия
                image.Mutate(ctx => ctx.Fill(
                    new DrawingOptions { GraphicsOptions = new GraphicsOptions { Antialias = true } },
                    markerBrush,
                    new EllipsePolygon(new PointF(sectorPoint.Pixel.X + padding, sectorPoint.Pixel.Y + padding), 3f)
                ));

                // Белая обводка
                image.Mutate(ctx => ctx.Draw(
                    new DrawingOptions { GraphicsOptions = new GraphicsOptions { Antialias = true } },
                    Pens.Solid(Color.White, 2),
                    new EllipsePolygon(new PointF(sectorPoint.Pixel.X + padding, sectorPoint.Pixel.Y + padding), 3f)
                ));

                //// Текст
                //image.Mutate(ctx => ctx.DrawText(new RichTextOptions(_font)
                //{
                //    Origin = new PointF((sectorPoint.Pixel.X) + 3 + padding, sectorPoint.Pixel.Y + padding), // смещение на 3 пикселя по ширине
                //    HorizontalAlignment = HorizontalAlignment.Left,
                //    VerticalAlignment = VerticalAlignment.Center
                //}, sectorPoint.ToString(), new SolidBrush(color)));
            }
            return image;
        }

        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
