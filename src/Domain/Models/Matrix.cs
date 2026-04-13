namespace Domain.Models
{
    public class Matrix
    {
        /// <summary>
        /// Словарь вершин и маршрутов
        /// </summary>
        public readonly Dictionary<PixelPoint, List<Line>> NodesAndPaths = [];
        /// <summary>
        /// Ширина в пикселях
        /// </summary>
        public int Width { get; }
        /// <summary>
        /// Высота в пикселях
        /// </summary>
        public int Height { get; }


        public Matrix(int width, int height, int n)
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
            var sidePoints = new List<PixelPoint>();
            int stepPoint = 2 * (Width + Height) / n + 1;

            // Вычисляем вершины
            for (int i = 1; i * stepPoint < Height; i++)
            {
                sidePoints.Add(new(0, i * stepPoint, i));
                sidePoints.Add(new(Width - 1, i * stepPoint, i));
            }
            for (int j = 1; j * stepPoint < Width; j++)
            {
                sidePoints.Add(new(j * stepPoint, 0, j));
                sidePoints.Add(new(j * stepPoint, Height - 1, j));
            }

            // Генерируем маршруты
            foreach (var start in sidePoints)
            {
                NodesAndPaths[start] = [];
                foreach (var end in sidePoints)
                {
                    if (start != end && start.X != end.X && start.Y != end.Y)
                    {
                        NodesAndPaths[start].Add(new(start, end));
                    }
                }
            }
        }

        /// <summary>
        /// Преобразует пиксельные координаты вершин в секторные
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public SectorPoint ConvertToCoordinate(PixelPoint p, int padding)
        {
            char sectorLetter = 'U';
            if (p.X == 0)
                sectorLetter = 'L';
            if (p.X == Width - 1)
                sectorLetter = 'R';
            if (p.Y == 0)
                sectorLetter = 'T';
            if (p.Y == Height - 1)
                sectorLetter = 'B';
            return new(sectorLetter, p.Number, new(p.X + padding, p.Y + padding));
        }


        /// <summary>
        /// Строит маршрут линий
        /// </summary>
        /// <param name="start">Стартовая вершина</param>
        /// <param name="negativeSourceMatrix">Матрица яркости пикселей исходного изображения (в негативе)</param>
        /// <param name="lineContrast">Значение контрастности линий при отрисовке</param>
        /// <returns>Маршрут (последовательный список линий)</returns>
        public async Task<List<Line>> BuildRoute(PixelPoint start, double[,] negativeSourceMatrix, double lineContrast, int stepCount)
        {
            if (!NodesAndPaths.TryGetValue(start, out var paths))
                throw new Exception("Не найдена стартовая вершина маршрута!");

            List<Line> route = [];
            for (int step = 0; step < stepCount; step++)
            {
                var line = await FindNextLine(start, route, negativeSourceMatrix);
                route.Add(line);
                foreach (var p in line.Points)
                {
                    negativeSourceMatrix[p.X, p.Y] -= lineContrast;

                }
                start = line.Points.Last();
            }
            return route;
        }


        /// <summary>
        /// Ищет лучший путь из заданной вершины (с самым высоким средним значением цвета на пиксель)
        /// </summary>
        /// <param name="start">Вершина из которой выполняется поиск</param>
        /// <returns></returns>
        private async Task<Line> FindNextLine(PixelPoint start, List<Line> route, double[,] negativeSourceMatrix)
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

        public PixelPoint SelectBeginPoint()
        {
            var keys = NodesAndPaths.Keys.ToList();
            return keys[new Random().Next(NodesAndPaths.Count)];
        }

        /// <summary>
        /// Проецирует маршрут линий на матрицу отрисовки
        /// </summary>
        /// <param name="route">Маршрут</param>
        /// <returns>Матрица отрисовки</returns>
        public async Task<double[,]> RenderRoute(List<Line> route)
        {
            double[,] renderingMatrix = new double[Width, Height];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    renderingMatrix[i, j] = 0;

            double maxValue = 0;
            foreach (var line in route)
            {
                foreach (var point in line.Points)
                {
                    var value = renderingMatrix[point.X, point.Y] + 1;
                    if (point.X != 0 && point.X != Width - 1 && point.Y != 0 && point.Y !=  Height - 1) // Пропускаем вершины, как самые плотные узлы
                        maxValue = Math.Max(maxValue, value);
                    renderingMatrix[point.X, point.Y] += 1;
                }
            }

            // Нормализуем до битовых значений
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    renderingMatrix[i, j] = 255 * Math.Clamp(renderingMatrix[i, j], 0, 255) / maxValue;
                }
            return renderingMatrix;
        }
    }
}
