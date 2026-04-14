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
            int stepPoint = 2 * (Width + Height) / n + 1;

            // Вычисляем вершины
            for (int i = 1; i * stepPoint < Height; i++)
            {
                results.Add(new(new(0, i * stepPoint, i), Width, Height));
                results.Add(new(new(Width - 1, i * stepPoint, i), Width, Height));
            }
            for (int j = 1; j * stepPoint < Width; j++)
            {
                results.Add(new(new(j * stepPoint, 0, j), Width, Height));
                results.Add(new(new(j * stepPoint, Height - 1, j), Width, Height));
            }
            return results;
        }

        private void AddPath(SectorPoint start, SectorPoint end)
        {
            if (start != end && start.Pixel.X != end.Pixel.X && start.Pixel.Y != end.Pixel.Y)
            {
                Paths[start].Add(new(start, end));
            }
        }
    }
}
