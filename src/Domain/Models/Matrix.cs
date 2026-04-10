using System;
using System.Collections.Generic;
using System.Text;

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
        public int Width;
        /// <summary>
        /// Высота в пикселях
        /// </summary>
        public int Height;
        /// <summary>
        /// Тип матрицы (эллипс / прямоугольник)
        /// </summary>
        public bool IsEllipse;


        public Matrix(int width, int height, int n, bool isEllipseMatrix)
        {
            IsEllipse = isEllipseMatrix;
            Width = width;
            Height = height;
            
            NodesAndPaths = CreatePathDictionary(n);
        }
        /// <summary>
        /// Создает словарь возможных маршрутов из каждой точки по периметру изображения
        /// </summary>
        /// <param name="width">Ширина изображения в пикселях</param>
        /// <param name="height">Высота изображения в пикселях</param>
        /// <param name="n">Число точек по периметру</param>
        /// <param name="isEllipseMatrix">Тип матрицы (круг / прямоугольник)</param>
        /// <returns></returns>
        private Dictionary<PixelPoint, List<Line>> CreatePathDictionary(int n)
        {
            var sidePoints = new List<PixelPoint>();
            int stepPixel = 2 * (Height + Width) / n;

            // Вычисляем вершины
            if (IsEllipse)
            {
                double a = Width / 2.0 - 1;
                double b = Height / 2.0 - 1;
                for (double angle = 0; angle < 2 * Math.PI; angle += 2 * Math.PI / n)
                {
                    int x = Math.Clamp((int)(a * Math.Cos(angle) + a), 0, Width - 1);
                    int y = Math.Clamp((int)(b * Math.Sin(angle) + b), 0, Height - 1);
                    sidePoints.Add(new(y, x));
                }
            }
            else
            {
                for (int i = 0; i < Width; i += stepPixel)
                {
                    sidePoints.Add(new(0, i));
                    sidePoints.Add(new(Height - 1, i));
                }
                for (int j = 0; j < Height; j += stepPixel)
                {
                    sidePoints.Add(new(j, 0));
                    sidePoints.Add(new(j, Width - 1));
                }
            }

            // Генерируем маршруты
            var paths = new Dictionary<PixelPoint, List<Line>>();
            foreach (var start in sidePoints)
            {
                paths[start] = [];
                foreach (var end in sidePoints)
                {
                    if (start != end && (IsEllipse || !(
                        (start.X == 0 && end.X == 0) ||
                        (start.X == Height - 1 && end.X == Height - 1) ||
                        (start.Y == 0 && end.Y == 0) ||
                        (start.Y == Width - 1 && end.Y == Width - 1)
                    )))
                    {
                        paths[start].Add(new(start, end));
                    }
                }
            }
            return paths;
        }

        /// <summary>
        /// Преобразует пиксельные координаты вершин в секторные
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public SectorPoint ConvertToCoordinate(PixelPoint p)
        {
            // Разделяем эллипс на 4 сектора
            int index = NodesAndPaths.Keys.ToList().IndexOf(p);
            int pointsPerSector = NodesAndPaths.Count / 4;
            int sector = index / pointsPerSector;
            int positionInSector = index % pointsPerSector;
            char sectorLetter;
            if (IsEllipse)
                sectorLetter = sector switch
                {
                    0 => 'A', // Правая часть (0° - 90°)
                    1 => 'B', // Верхняя часть (90° - 180°)
                    2 => 'C', // Левая часть (180° - 270°)
                    3 => 'D', // Нижняя часть (270° - 360°)
                    _ => 'A'
                };
            else
                sectorLetter = sector switch
                {
                    0 => 'T',
                    1 => 'R',
                    2 => 'B',
                    3 => 'L',
                    _ => 'T'
                };
            return new(sectorLetter, positionInSector + 1); 
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
    }
}
