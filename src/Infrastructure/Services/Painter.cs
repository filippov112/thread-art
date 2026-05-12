using Domain.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services
{
    public class Painter
    {
        public static void DrawMatrix(Image<Rgba32> image, int[] values)
        {
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    var value = (byte)values[y * image.Width + x];
                    image[x, y] = new(value, value, value);
                }
        }

        public static void DrawCoordinateGrid(Image<Rgba32> image, int padding, SectorPoint[] points)
        {
            //Font _font = SystemFonts.CreateFont("Arial", 8);
            Dictionary<char, Color> colors = new() {
                {'A', Color.Red},
                {'B', Color.Blue},
                {'C', Color.Green},
                {'D', Color.Purple},
                {'T', Color.Red},
                {'R', Color.Green},
                {'L', Color.Purple}
            };
            foreach (SectorPoint sectorPoint in points)
            {
                var color = colors.TryGetValue(sectorPoint.Sector, out Color value) ? value : Color.Black;
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
        }
    }
}
