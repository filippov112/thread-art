using Domain.Models;

namespace Domain.Services
{
    public class PointsFinder
    {
        public static SectorPoint[] GetPoints(int width, int height, int n)
        {
            if (n < 2 || width < 3 || height < 3)
                return [];

            n = Math.Min(n, 2 * (width + height) - 8); // Не разрешаем число точек большее, чем число пикселей
            var selectedPoints = FindPoints(width, height, n);
            List<SectorPoint> results = [];

            // Вычисляем вершины
            int counterT = 0;
            for (int j = 1; j < width - 1; j++) // top
                if (selectedPoints.Contains(new(j, 0)))
                    results.Add(new(new(j, 0), width, height, counterT + 1));
            int counterR = 0;
            for (int j = 1; j < height - 1; j++) // right
                if (selectedPoints.Contains(new(width - 1, j)))
                    results.Add(new(new(width - 1, j), width, height, counterR + 1));
            int counterB = 0;
            for (int j = 1; j < width - 1; j++) // bottom
                if (selectedPoints.Contains(new(width - 1 - j, height - 1)))
                    results.Add(new(new(width - 1 - j, height - 1), width, height, counterB + 1));
            int counterL = 0;
            for (int j = 1; j < height - 1; j++) // left
                if (selectedPoints.Contains(new(0, height - 1 - j)))
                    results.Add(new(new(0, height - 1 - j), width, height, counterL + 1));
            return [.. results];
        }

        /// <summary>
        /// Находит вершины на пересечении лучей с границей изображения
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static HashSet<PixelPoint> FindPoints(int width, int height, int n)
        {
            HashSet<PixelPoint> selectedPoints = [];
            // Находим центр
            var max = Math.Max(width, height);
            double x0 = (int)Math.Round(width / 2.0);
            double y0 = (int)Math.Round(height / 2.0);
            for (double angle = 0; angle < 2 * Math.PI; angle += 2 * Math.PI / n)
            {
                int x = (int)(2 * max * Math.Cos(angle) + x0);
                int y = (int)(2 * max * Math.Sin(angle) + y0);
                IEnumerable<PixelPoint> line = LineConstructor.GetLineIterator(
                    new((int)x0, (int)y0),
                    new(x, y)
                );
                foreach (var point in line) // Выбираем первую точку, которая попадает на край изображения
                {
                    if ((point.X == 0 || point.X == width - 1 || point.Y == 0 || point.Y == height - 1) // Определяем по крайнему значению любой из координат
                        && !(point.X == 0 && point.Y == 0) && !(point.X == width - 1 && point.Y == height - 1) // Исключаем углы главной диагонали
                        && !(point.X == 0 && point.Y == height - 1) && !(point.X == width - 1 && point.Y == 0) // Исключаем углы побочной диагонали
                    )
                    {
                        selectedPoints.Add(point);
                        break;
                    }
                }
            }
            return selectedPoints;
        }
    }
}
