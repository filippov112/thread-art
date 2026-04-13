using Application.Services;
using Domain.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure
{
    public class Painter : IPainter
    {
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

        private readonly Font _font = SystemFonts.CreateFont("Arial", 8);

        public async Task<PixelMatrix> GetPixelMatrix(string inputImagePath)
        {
            var image = await Image.LoadAsync<Rgba32>(inputImagePath);
            var matrix = new double[image.Width, image.Height];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    matrix[x, y] = 255 - (image[x, y].R + image[x, y].G + image[x, y].B) / 3;
                }
            }
            return new(matrix);
        }

        private Image<Rgba32> RestoreImage(RouteMatrix smallmatrix, PixelMatrix largeMatrix)
        {
            int padding = (int)(Math.Max(smallmatrix.Width, smallmatrix.Height) * 0.05f);
            Image<Rgba32> image = new(largeMatrix.Width, largeMatrix.Height, new Rgba32(255, 255, 255, 255));

            for (int x = 0; x < smallmatrix.Width; x++)
            {
                for (int y = 0; y < smallmatrix.Height; y++)
                {
                    var value = (byte)largeMatrix.Values[x + padding, y + padding];
                    image[x + padding, y + padding] = new(value, value, value);
                }
            }
            return image;
        }

        /// <summary>
        /// Отрисовывает изображение
        /// </summary>
        /// <param name="values">Значения яркости пикселей (негатив)</param>
        /// <param name="padding">Отступы по краям</param>
        /// <returns></returns>
        public async Task<PixelMatrix> DrawImage(RouteMatrix matrix, List<Line> route)
        {
            int padding = (int)(Math.Max(matrix.Width, matrix.Height) * 0.05f);

            var values = await matrix.GetRenderImage(route);

            int widthPlus = values.GetLength(0) + padding * 2;
            int heightPlus = values.GetLength(1) + padding * 2;

            var pixelMatrix = new PixelMatrix(widthPlus, heightPlus);
            for (int i = 0; i < matrix.Width; i++)
                for (int j = 0; j < matrix.Height; j++)
                {
                    int newValue = 255 - (int)values[i, j];
                    pixelMatrix.Values[i + padding, j + padding] = newValue;
                }
            return pixelMatrix;
        }

        private Image<Rgba32> DrawCoordinateGrid(RouteMatrix matrix, PixelMatrix pixelMatrix)
        {
            var image = RestoreImage(matrix, pixelMatrix);
            int padding = (int)(Math.Max(matrix.Width, matrix.Height) * 0.05f);
            foreach (SectorPoint sectorPoint in matrix.NodesAndPaths.Keys)
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

                // Текст
                image.Mutate(ctx => ctx.DrawText(new RichTextOptions(_font)
                {
                    Origin = new PointF((sectorPoint.Pixel.X) + 3 + padding, sectorPoint.Pixel.Y + padding), // смещение на 3 пикселя по ширине
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                }, sectorPoint.ToString(), new SolidBrush(color)));
            }
            return image;
        }

        public async Task SaveImage(string path, RouteMatrix matrix, PixelMatrix pixelMatrix)
        {
            var image = DrawCoordinateGrid(matrix, pixelMatrix);
            image.Save(path);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
