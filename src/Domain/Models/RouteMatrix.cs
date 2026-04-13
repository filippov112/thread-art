namespace Domain.Models
{
    public class RouteMatrix
    {
        /// <summary>
        /// Словарь вершин и маршрутов
        /// </summary>
        public readonly Dictionary<SectorPoint, List<Line>> NodesAndPaths = [];
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

            CreatePathDictionary(n);
        }
        /// <summary>
        /// Создает словарь возможных маршрутов из каждой точки по периметру изображения
        /// </summary>
        /// <param name="width">Ширина изображения в пикселях</param>
        /// <param name="height">Высота изображения в пикселях</param>
        /// <param name="n">Число точек по периметру</param>
        /// <param name="isEllipseMatrix">Тип матрицы (круг / прямоугольник)</param>
        /// <returns></returns>
        private void CreatePathDictionary(int n)
        {
            var size = new SizeImage(Width, Height);
            var sidePoints = new List<SectorPoint>();
            int stepPoint = 2 * (Width + Height) / n + 1;

            // Вычисляем вершины
            for (int i = 1; i * stepPoint < Height; i++)
            {
                sidePoints.Add(new(new(0, i * stepPoint, i), size));
                sidePoints.Add(new(new(Width - 1, i * stepPoint, i), size));
            }
            for (int j = 1; j * stepPoint < Width; j++)
            {
                sidePoints.Add(new(new(j * stepPoint, 0, j), size));
                sidePoints.Add(new(new(j * stepPoint, Height - 1, j), size));
            }

            // Генерируем маршруты

            foreach (var startSector in sidePoints)
            {
                NodesAndPaths[startSector] = [];
                foreach (var endSector in sidePoints)
                {
                    if (startSector != endSector && startSector.Pixel.X != endSector.Pixel.X && startSector.Pixel.Y != endSector.Pixel.Y)
                    {
                        NodesAndPaths[startSector].Add(new(startSector, endSector));
                    }
                }
            }
        }

        public async Task<double[,]> GetRenderImage(List<Line> route)
        {
            double[,] result = new double[Width, Height];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    result[i, j] = 0;

            await RenderRoute(result, route);
            var maxValue = await CalcContrast(result);

            // Нормализуем до битовых значений
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    result[i, j] = 255 * Math.Clamp(result[i, j], 0, 255) / maxValue;
                }
            return result;
        }

        /// <summary>
        /// Проецирует маршрут линий на матрицу отрисовки
        /// </summary>
        /// <param name="route">Маршрут</param>
        /// <returns>Матрица отрисовки</returns>
        private static async Task RenderRoute(double[,] matrix, List<Line> route)
        {
            foreach (var line in route)
                foreach (var point in line.Points)
                    matrix[point.X, point.Y] += 1;
        }

        private async Task<double> CalcContrast(double[,] matrix)
        {
            double maxValue = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    if (i != 0 && i != Width - 1 && j != 0 && j != Height - 1) // Пропускаем вершины, как самые плотные узлы
                        maxValue = Math.Max(maxValue, matrix[i, j]);
            return maxValue;
        }

        /// <summary>
        /// Строит маршрут линий
        /// </summary>
        /// <param name="start">Стартовая вершина</param>
        /// <param name="negativeSourceMatrix">Матрица яркости пикселей исходного изображения (в негативе)</param>
        /// <param name="lineContrast">Значение контрастности линий при отрисовке</param>
        /// <returns>Маршрут (последовательный список линий)</returns>
        public async Task<List<Line>> BuildRoute(SectorPoint start, PixelMatrix pixelMatrix, double lineContrast, int stepCount, Action<int>? onProgress = null)
        {
            if (!NodesAndPaths.TryGetValue(start, out var paths))
                throw new Exception("Не найдена стартовая вершина маршрута!");

            List<Line> route = [];
            for (int step = 0; step < stepCount; step++)
            {
                var line = await FindNextLine(start, route, pixelMatrix.Values);
                route.Add(line);
                foreach (var p in line.Points)
                {
                    pixelMatrix.Values[p.X, p.Y] -= lineContrast;
                }
                start = line.End;

                if (onProgress != null && step % Math.Max(1, stepCount / 100) == 0)
                {
                    int percent = (int)((step + 1) * 100.0 / stepCount);
                    onProgress.Invoke(percent);
                }
            }
            onProgress?.Invoke(100);
            return route;
        }


        /// <summary>
        /// Ищет лучший путь из заданной вершины (с самым высоким средним значением цвета на пиксель)
        /// </summary>
        /// <param name="start">Вершина из которой выполняется поиск</param>
        /// <returns></returns>
        private async Task<Line> FindNextLine(SectorPoint start, List<Line> route, double[,] negativeSourceMatrix)
        {
            double maxTotal = double.MinValue;
            Line bestPath = NodesAndPaths[start].First();

            foreach (var path in NodesAndPaths[start])
            {
                double sum = 0;
                int count = 0;

                foreach (var p in path.Points)
                {
                    sum += negativeSourceMatrix[p.X, p.Y];
                    count++;
                }

                if (count > 0)
                {
                    double avgProb = sum / count;
                    // Запрещаем возвращаться по тому же маршруту
                    if (route.Count > 0 && route.Last().IsRevert(path))
                        continue;
                    if (avgProb > maxTotal)
                    {
                        maxTotal = avgProb;
                        bestPath = path;
                    }
                }
            }
            return bestPath;
        }

        public SectorPoint SelectBeginPoint()
        {
            var keys = NodesAndPaths.Keys.ToList();
            return keys[new Random().Next(NodesAndPaths.Count)];
        }


    }
}
