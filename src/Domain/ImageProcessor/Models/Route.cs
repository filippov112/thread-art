using Domain.ImageProcessor.Services;
using Domain.QueueManager.Services;

namespace Domain.ImageProcessor.Models;

/// <summary>
/// Маршрут отрисовки
/// </summary>
/// <param name="start">Стартовая точка</param>
public class Route
{
    public readonly List<SectorPoint> Points;
    public readonly ImageMatrix Matrix;
    private readonly int _contrastLine;

    public Route(int countSteps, ImageMatrix matrix, ProgressLogger logger)
    {
        Points = [matrix.SidePoints!.First()];
        Matrix = matrix;
        _contrastLine = CalculateOptimalContrast(countSteps);

        if (countSteps < 7) // Цель - раздробить процесс на промежутки ~10% (до 80%)
            FillRoute(countSteps);
        else
        {
            var progressStep = countSteps / 7;
            for (int i = progressStep; i <= countSteps; i = Math.Clamp(i + progressStep, 0, countSteps))
            {
                var batchSize = i % progressStep == 0 ? progressStep : i % progressStep;
                FillRoute(batchSize);
                Task.Run(async () => logger.UpdateProgress(10 + (int)Math.Round(70d * i / countSteps)));
                if (i == countSteps)
                    break;
            }
        }
    }

    /// <summary>
    /// Строит маршрут линий
    /// </summary>
    /// <param name="start">Стартовая вершина</param>
    /// <param name="negativeSourceMatrix">Матрица яркости пикселей исходного изображения (в негативе)</param>
    /// <param name="lineContrast">Значение контрастности линий при отрисовке</param>
    /// <returns>Маршрут (последовательный список линий)</returns>
    public void FillRoute(int stepCount)
    {
        // start - уже находится в route.Points
        for (int step = 0; step < stepCount; step++)
        {
            var point = FindNextPoint();
            if (point == null)
                continue;

            Points.Add((SectorPoint)point); // Сохраняем найденную точку в маршрут
        }
    }


    /// <summary>
    /// Находит следующую оптимальную точку маршрута
    /// </summary>
    /// <returns></returns>
    private SectorPoint? FindNextPoint()
    {
        double minValue = int.MaxValue;
        var directions = GetAllDirections();
        SectorPoint? bestDirection = directions.FirstOrDefault();
        if (bestDirection == null)
            return null;

        var lastPoint = Points.Last();
        foreach (var direction in directions)
        {
            double sum = 0;
            int count = 0;
            foreach (var pixel in BresenhamAlgorithm.GetLineIterator(lastPoint.Pixel, direction.Pixel))
            {
                sum += Matrix.Pixels![pixel.Y * Matrix.Width + pixel.X];
                count++;
            }

            if (count > 0)
            {
                double avgProb = sum / count;
                // Запрещаем возвращаться по тому же маршруту
                if (Points.Count > 1 && Points[^2] == direction)
                    continue;
                if (avgProb < minValue)
                {
                    minValue = avgProb;
                    bestDirection = direction;
                }
            }
        }
        foreach (var pixel in BresenhamAlgorithm.GetLineIterator(lastPoint.Pixel, ((SectorPoint)bestDirection).Pixel))
        {
            Matrix.Pixels![pixel.Y * Matrix.Width + pixel.X] += _contrastLine;
        }
        return bestDirection;
    }

    /// <summary>
    /// Все возможные направления из текущей конечной точки маршрута
    /// </summary>
    /// <returns></returns>
    private List<SectorPoint> GetAllDirections()
    {
        List<SectorPoint> points = [];
        var lastPoint = Points.Last();
        foreach (var point in Matrix.SidePoints!)
            if (lastPoint.Pixel.X != point.Pixel.X && lastPoint.Pixel.Y != point.Pixel.Y)
                points.Add(point);
        return points;
    }

    /// <summary>
    /// Вычисляет оптимальный вес одной линии для моделирования изображения
    /// </summary>
    /// <param name="stepCount"></param>
    /// <returns></returns>
    public int CalculateOptimalContrast(int stepCount)
    {
        int width = Matrix.Width;
        int height = Matrix.Height;

        // 1. Площадь и диагональ
        double s = width * height;
        double d = Math.Sqrt(width * width + height * height);

        // 2. Стандартное отклонение (Контраст матрицы)
        // Вычисляем среднее
        double sum = 0;
        foreach (var p in Matrix.Pixels!)
            sum += p;
        double mean = sum / Matrix.Pixels.Length;

        // Вычисляем дисперсию
        double varianceSum = 0;
        foreach (var p in Matrix.Pixels)
        {
            double diff = p - mean;
            varianceSum += diff * diff;
        }
        double variance = varianceSum / Matrix.Pixels.Length;
        double sigma = Math.Sqrt(variance); // Стандартное отклонение

        // 3. Формула
        // x = 3.2 * (2 * S * sigma) / (N * D)
        // 3.2 - произвольный коэффициент
        double rawX = (6.4 * s * sigma) / (stepCount * d);

        // Округляем до целого
        return (int)Math.Round(rawX);
    }
}
