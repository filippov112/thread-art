namespace Domain.Models
{
    public class RouteMatrix
    {
        public SectorPoint[] Points;
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
            Points = GetPoints(n);
        }

        private SectorPoint[] GetPoints(int n)
        {
            if (n < 2 || Width < 3 || Height < 3)
                return [];

            n = Math.Min(n, 2 * (Width + Height) - 8); // Не разрешаем число точек большее, чем число пикселей
            var selectedPoints = FindPoints(n);
            var results = new SectorPoint[selectedPoints.Count];

            // Вычисляем вершины
            int counterT = 0;
            for (int j = 1; j < Width - 1; j++) // top
                if (selectedPoints.Contains(new(j, 0)))
                    results[counterT++] = new(new(j, 0, counterT + 1), Width, Height);
            int counterR = 0;
            for (int j = 1; j < Height - 1; j++) // right
                if (selectedPoints.Contains(new(Width - 1, j)))
                    results[counterT + counterR++] = new(new(Width - 1, j, counterR + 1), Width, Height);
            int counterB = 0;
            for (int j = 1; j < Width - 1; j++) // bottom
                if (selectedPoints.Contains(new(Width - 1 - j, Height - 1)))
                    results[counterT + counterR + counterB++] = new(new(Width - 1 - j, Height - 1, counterB + 1), Width, Height);
            int counterL = 0;
            for (int j = 1; j < Height - 1; j++) // left
                if (selectedPoints.Contains(new(0, Height - 1 - j)))
                    results[counterT + counterR + counterB + counterL++] = new(new(0, Height - 1 - j, counterL + 1), Width, Height);
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
                IEnumerable<PixelPoint> line = Line.GetLineIterator(
                    new((int)x0, (int)y0),
                    new(x, y)
                );
                foreach (var point in line) // Выбираем первую точку, которая попадает на край изображения
                {
                    if ((point.X == 0 || point.X == Width - 1 || point.Y == 0 || point.Y == Height - 1) // Определяем по крайнему значению любой из координат
                        && !(point.X == 0 && point.Y == 0) && !(point.X == Width - 1 && point.Y == Height - 1) // Исключаем углы главной диагонали
                        && !(point.X == 0 && point.Y == Height - 1) && !(point.X == Width - 1 && point.Y == 0) // Исключаем углы побочной диагонали
                    )
                    {
                        selectedPoints.Add(point);
                        break;
                    }
                }
            }
            return selectedPoints;
        }

        public List<SectorPoint> GetLineEndPoints(SectorPoint start)
        {
            List<SectorPoint> points = [];
            foreach(var point in Points)
                if (start.Pixel.X != point.Pixel.X && start.Pixel.Y != point.Pixel.Y)
                    points.Add(point);
            return points;
        }
    }
}
