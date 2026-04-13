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

        private Image<Rgba32>? _image;
        public SizeImage? Size => _image != null ? new SizeImage(_image.Width, _image.Height) : null;


        public async Task<double[,]> GetImageGrayNegativeMatrix(string inputImagePath)
        {
            _image = await Image.LoadAsync<Rgba32>(inputImagePath);
            var matrix = new double[_image.Width, _image.Height];
            for (int x = 0; x < _image.Width; x++)
            {
                for (int y = 0; y < _image.Height; y++)
                {
                    matrix[x, y] = 255 - (_image[x, y].R + _image[x, y].G + _image[x, y].B) / 3;
                }
            }
            return matrix;
        }

        /// <summary>
        /// Отрисовывает изображение
        /// </summary>
        /// <param name="values">Значения яркости пикселей (негатив)</param>
        /// <param name="padding">Отступы по краям</param>
        /// <returns></returns>
        public async Task DrawImage(double[,] values, int padding)
        {
            _image = new(values.GetLength(0) + padding * 2, values.GetLength(1) + padding * 2);
            for (int i = 0; i < values.GetLength(0); i++)
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    int newValue = 255 - (int)values[i, j];
                    _image[i + padding, j + padding] = new Rgba32((byte)newValue, (byte)newValue, (byte)newValue);
                }

        }

        /// <summary>
        /// Отрисовывает метку координаты
        /// </summary>
        /// <param name="imagePoint"></param>
        /// <param name="point"></param>
        public void DrawCoordinate(SectorPoint point)
        {
            if (_image == null)
                return;
            var color = _colors.TryGetValue(point.Sector, out Color value) ? value : Color.Black;
            var markerBrush = new SolidBrush(color);

            // Рисуем маркер точки (круг) - исправленная версия
            _image.Mutate(ctx => ctx.Fill(
                new DrawingOptions { GraphicsOptions = new GraphicsOptions { Antialias = true } },
                markerBrush,
                new EllipsePolygon(new PointF(point.Pixel?.X ?? 0, point.Pixel?.Y ?? 0), 3f)
            ));

            // Белая обводка
            _image.Mutate(ctx => ctx.Draw(
                new DrawingOptions { GraphicsOptions = new GraphicsOptions { Antialias = true } },
                Pens.Solid(Color.White, 2),
                new EllipsePolygon(new PointF(point.Pixel?.X ?? 0, point.Pixel?.Y ?? 0), 3f)
            ));

            // Текст
            _image.Mutate(ctx => ctx.DrawText(new RichTextOptions(_font)
            {
                Origin = new PointF((point.Pixel?.X ?? 0) + 3, point.Pixel?.Y ?? 0), // смещение на 3 пикселя по ширине
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            }, point.ToString(), new SolidBrush(color)));
        }



        public async Task SaveImage(string path)
        {
            _image?.Save(path);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
