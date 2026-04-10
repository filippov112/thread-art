using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Domain.Models
{
    public class Canvas
    {
        /// <summary>
        /// Малая матрица (расчетная)
        /// </summary>
        public Matrix SmallMatrix { get; set; }
        /// <summary>
        /// Большая матрица (отрисовки)
        /// </summary>
        public Matrix LargeMatrix { get; set; }
        /// <summary>
        /// Соотношение вершин между матрицами (для проекции маршрута)
        /// </summary>
        public Dictionary<PixelPoint, PixelPoint> SmallToLargeNodes = [];

        public Canvas(Matrix matrix, int width, int height) 
        {
            SmallMatrix = matrix;
            LargeMatrix = new(width, height, matrix.NodesAndPaths.Count, matrix.IsEllipse);

            CorrelateMatrixNodes();
        }

    

        /// <summary>
        /// Заполняет SmallToLargeNodes (соотносит вершины матриц)
        /// </summary>
        private void CorrelateMatrixNodes()
        {
            var smallKeys = SmallMatrix.NodesAndPaths.Keys.ToArray();
            var largeKeys = LargeMatrix.NodesAndPaths.Keys.ToArray();
            for (int i = 0; i < smallKeys.Length; i++)
            {
                SmallToLargeNodes[smallKeys[i]] = largeKeys[i];
            }
        }

        /// <summary>
        /// Проецирует маршрут линий на матрицу отрисовки
        /// </summary>
        /// <param name="route">Маршрут</param>
        /// <returns>Матрица отрисовки</returns>
        public async Task<double[,]> ProjectRouteOntoRenderingMatrix(List<Line> route)
        {
            double[,] renderingMatrix = new double[LargeMatrix.Width, LargeMatrix.Height];
            for (int i = 0; i < LargeMatrix.Width; i++)
                for (int j = 0; j < LargeMatrix.Height; j++)
                    renderingMatrix[i, j] = 0;

            double maxValue = 0;
            foreach (var line in route)
            {
                // Конвертируем координаты начала и конца линии и находим соответствующий маршрут в словаре большой матрицы
                var largeLine = LargeMatrix.NodesAndPaths[SmallToLargeNodes[line.Points.First()]].First(path => path.Points.Last() == SmallToLargeNodes[line.Points.Last()]);
                foreach(var point in largeLine.Points)
                {
                    var value = renderingMatrix[point.X, point.Y] + 1;
                    maxValue = Math.Max(maxValue, value);
                    renderingMatrix[point.X, point.Y] += 1;
                }
            }

            // Нормализуем до битовых значений
            for(int i = 0; i < LargeMatrix.Width; i++)
                for(int j = 0; j < LargeMatrix.Height; j++)
                {
                    renderingMatrix[i, j] = 255 * renderingMatrix[i, j] / maxValue; 
                }
            return renderingMatrix;
        }

        
    }
}
