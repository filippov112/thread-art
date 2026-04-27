namespace Domain.Models
{
    public class RouteMatrix
    {
        public SectorPoint[] Points => [.. Paths.Keys];
        /// <summary>
        /// Словарь вершин и маршрутов
        /// </summary>
        public readonly Dictionary<SectorPoint, List<Line>> Paths = [];
        /// <summary>
        /// Ширина в пикселях
        /// </summary>
        public int Width { get; }
        /// <summary>
        /// Высота в пикселях
        /// </summary>
        public int Height { get; }

        public RouteMatrix(int width, int height, int n)
        {
            Width = width;
            Height = height;
            var sidePoints = GetPoints(n);

            // Генерируем маршруты
            foreach (var startSector in sidePoints)
            {
                Paths[startSector] = [];
                foreach (var endSector in sidePoints)
                    AddPath(startSector, endSector);
                if (Paths[startSector].Count == 0)
                    Paths.Remove(startSector);
            }
        }

        private List<SectorPoint> GetPoints(int n)
        {
            var results = new List<SectorPoint>();
            if (n == 0 || Width < 3 || Height < 3)
                return results;

            n = Math.Min(n, 2 * (Width + Height) - 8); // Не разрешаем число точек большее, чем число пикселей
            var selectedPoints = FindPoints(n);

            // Вычисляем вершины
            int counter = 0;
            for (int j = 1; j < Width - 1; j++) // top
            {
                if (selectedPoints.Contains(new(j, 0)))
                {
                    results.Add(new(new(j, 0, counter + 1), Width, Height));
                    counter++;
                }
            }
            counter = 0;
            for (int j = 1; j < Height - 1; j++) // right
            {
                if (selectedPoints.Contains(new(Width - 1, j)))
                {
                    results.Add(new(new(Width - 1, j, counter + 1), Width, Height));
                    counter++;
                }
            }
            counter = 0;
            for (int j = 1; j < Width - 1; j++) // bottom
            {
                if (selectedPoints.Contains(new(Width - 1 - j, Height - 1)))
                {
                    results.Add(new(new(Width - 1 - j, Height - 1, counter + 1), Width, Height));
                    counter++;
                }
            }
            counter = 0;
            for (int j = 1; j < Height - 1; j++) // left
            {
                if (selectedPoints.Contains(new(0, Height - 1 - j)))
                {
                    results.Add(new(new(0, Height - 1 - j, counter + 1), Width, Height));
                    counter++;
                }
            }
            return results;
        }

        /// <summary>
        /// Находит вершины на пересечении лучей с границей изображения
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private HashSet<PixelPoint> FindPoints(int n)
        {
            HashSet<PixelPoint> selectedPoints = [];
            // Находим центр
            var max = Math.Max(Width, Height);
            double x0 = (int)Math.Round(Width / 2.0);
            double y0 = (int)Math.Round(Height / 2.0);
            for (double angle = 0; angle < 2 * Math.PI; angle += 2 * Math.PI / n)
            {
                int x = (int)(2 * max * Math.Cos(angle) + x0);
                int y = (int)(2 * max * Math.Sin(angle) + y0);
                Line line = new(
                    new(new((int)x0, (int)y0), Width, Height),
                    new(new(x, y), Width, Height)
                );
                foreach (var point in line.Points) // Выбираем первую точку, которая попадает на край изображения
                {
                    if ((point.X == 0 || point.X == Width - 1 || point.Y == 0 || point.Y == Height - 1) // Определяем по крайнему значению любой из координат
                        && point.X != point.Y // Исключаем углы главной диагонали
                        && point.Y - point.X < Height - 1 // Исключаем углы второй диагонали
                        && point.X - point.Y < Width - 1
                        )
                    {
                        selectedPoints.Add(point);
                        break;
                    }
                }
            }
            return selectedPoints;
        }

        private void AddPath(SectorPoint start, SectorPoint end)
        {
            if (start.Pixel.X != end.Pixel.X && start.Pixel.Y != end.Pixel.Y)
            {
                Paths[start].Add(new(start, end));
            }
        }
    }
}
