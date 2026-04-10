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
    public class Painter: IPainter
    {
        // Цвета для разных секторов/сторон
        private Dictionary<char, Color> Colors = new() {
                {'A', Color.Red},
                {'B', Color.Blue},
                {'C', Color.Green},
                {'D', Color.Purple},
                {'T', Color.Red},
                {'R', Color.Green},
                {'L', Color.Purple}
            };

        private Font font = SystemFonts.CreateFont("Arial", 8);

        private Image<Rgba32> BaseImage;
        private Image<Rgba32> SmallImage;
        private Image<Rgba32> RenderImage;


        public async Task<double[,]> GetImageGrayNegativeMatrix(string inputImagePath, SizeImage smallSize)
        {
            BaseImage = await Image.LoadAsync<Rgba32>(inputImagePath);
            SmallImage = BaseImage.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(smallSize.Width, smallSize.Height),
                Mode = ResizeMode.Crop, // Обрезаем изображение вместо деформации
                Position = AnchorPositionMode.Center // Центрируем изображение перед обрезкой
            }));

            var smallMatrix = new double[smallSize.Width, smallSize.Height];
            for (int x = 0; x < smallSize.Width; x++)
            {
                for (int y = 0; y < smallSize.Height; y++)
                {
                    smallMatrix[x, y] = 255 - (SmallImage[x, y].R + SmallImage[x, y].R + SmallImage[x, y].R) / 3;
                }
            }
            return smallMatrix;
        }

        /// <summary>
        /// Отрисовывает изображение
        /// </summary>
        /// <param name="values">Значения яркости пикселей (негатив)</param>
        /// <param name="padding">Отступы по краям</param>
        /// <returns></returns>
        public async Task DrawImage(double[,] values, int padding)
        {
            RenderImage = new(values.GetLength(0) + padding * 2, values.GetLength(1) + padding * 2);
            for (int i = 0; i < values.GetLength(0); i++)
                for(int j = 0;  j < values.GetLength(1); j++)
                {
                    int newValue = 255 - (int)values[i, j];
                    RenderImage[i + padding, j + padding] = new Rgba32((byte)newValue, (byte)newValue, (byte)newValue);
                }

        }

        /// <summary>
        /// Отрисовывает метку координаты
        /// </summary>
        /// <param name="imagePoint"></param>
        /// <param name="sectorPoint"></param>
        public void DrawCoordinate(PixelPoint imagePoint, SectorPoint sectorPoint)
        {
            var color = Colors.TryGetValue(sectorPoint.Sector, out Color value) ? value : Color.Black;
            var markerBrush = new SolidBrush(color);

            // Рисуем маркер точки (круг) - исправленная версия
            RenderImage.Mutate(ctx => ctx.Fill(
                new DrawingOptions { GraphicsOptions = new GraphicsOptions { Antialias = true } },
                markerBrush,
                new EllipsePolygon(new PointF(imagePoint.X, imagePoint.Y), 3f)
            ));

            // Белая обводка
            RenderImage.Mutate(ctx => ctx.Draw(
                new DrawingOptions { GraphicsOptions = new GraphicsOptions { Antialias = true } },
                Pens.Solid(Color.White, 2),
                new EllipsePolygon(new PointF(imagePoint.X, imagePoint.Y), 3f)
            ));

            // Текст
            RenderImage.Mutate(ctx => ctx.DrawText(new RichTextOptions(font)
            {
                Origin = new PointF(imagePoint.X + 3, imagePoint.Y), // смещение на 3 пикселя по ширине
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            }, sectorPoint.ToString(), new SolidBrush(color)));
        }

        

        public async Task SaveImage(string path)
        {
            RenderImage.Save(path);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
