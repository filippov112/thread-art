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
            }
        }

        private List<SectorPoint> GetPoints(int n)
        {
            var results = new List<SectorPoint>();
            if (n == 0 || Width < 3 || Height < 3)
                return results;

            n = Math.Min(n, 2 * (Width + Height) - 8); // Не разрешаем число точек большее, чем число пикселей
            double step = (double)(2 * (Width + Height) - 8) / (n + 1);

            // Вычисляем вершины
            int counter = 0;
            for (int j = 1; (int)Math.Round(j * step) < Width - 1 && counter < n; j++) // top
            {
                results.Add(new(new((int)Math.Round(j * step), 0, j), Width, Height));
                counter++;
            }
            for (int j = 1; (int)Math.Round(j * step) < Height - 1 && counter < n; j++) // right
            {
                results.Add(new(new(Width - 1, (int)Math.Round(j * step), j), Width, Height));
                counter++;
            }
            for (int j = 1; (int)Math.Round(j * step) < Width - 1 && counter < n; j++) // bottom
            {
                results.Add(new(new(Width - 1 - (int)Math.Round(j * step), Height - 1, j), Width, Height));
                counter++;
            }
            for (int j = 1; (int)Math.Round(j * step) < Height - 1 && counter < n; j++) // left
            {
                results.Add(new(new(0, Height - 1 - (int)Math.Round(j * step), j), Width, Height));
                counter++;
            }
            return results;
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
