using Domain.Models;

namespace Domain.Services;

public class RouteBuilder
{
    /// <summary>
    /// Строит маршрут линий
    /// </summary>
    /// <param name="start">Стартовая вершина</param>
    /// <param name="negativeSourceMatrix">Матрица яркости пикселей исходного изображения (в негативе)</param>
    /// <param name="lineContrast">Значение контрастности линий при отрисовке</param>
    /// <returns>Маршрут (последовательный список линий)</returns>
    public static SectorPoint FillRoute(SectorPoint start, SectorPoint[] points, Route route, ImageMatrix originalImage, int stepCount, int lineContrast)
    {
        // start - уже находится в route.Points
        for (int step = 0; step < stepCount; step++)
        {
            var point = FindNextPoint(points, start, route, originalImage, lineContrast);
            if (point == null)
                continue;
            start = (SectorPoint)point; // Обновляем start
            route.Points.Add(start); // Сохраняем найденную точку в маршрут
        }
        return start; // Возвращаем последнюю добавленную точку маршрута, с которой начнется следующий batch.
    }

    private static SectorPoint? FindNextPoint(SectorPoint[] points, SectorPoint start, Route route, ImageMatrix originalImage, int lineContrast)
    {
        double minValue = int.MaxValue;
        var ends = GetLineEndPoints(points, start);
        SectorPoint? bestEndPoint = ends.FirstOrDefault();
        if (bestEndPoint == null)
            return null;
        foreach (var end in ends)
        {
            double sum = 0;
            int count = 0;
            foreach (var p in LineConstructor.GetLineIterator(start.Pixel, end.Pixel))
            {
                sum += originalImage.Pixels[p.Y * originalImage.Width + p.X];
                count++;
            }

            if (count > 0)
            {
                double avgProb = sum / count;
                // Запрещаем возвращаться по тому же маршруту
                if (route.Points.Count > 1 && route.Points[^2] == end)
                    continue;
                if (avgProb < minValue)
                {
                    minValue = avgProb;
                    bestEndPoint = end;
                }
            }
        }
        foreach (var p in LineConstructor.GetLineIterator(start.Pixel, ((SectorPoint)bestEndPoint).Pixel))
        {
            originalImage.Pixels[p.Y * originalImage.Width + p.X] += lineContrast;
        }
        return bestEndPoint;
    }

    private static List<SectorPoint> GetLineEndPoints(SectorPoint[] all, SectorPoint start)
    {
        List<SectorPoint> points = [];
        foreach (var point in all)
            if (start.Pixel.X != point.Pixel.X && start.Pixel.Y != point.Pixel.Y)
                points.Add(point);
        return points;
    }

    public static int CalculateOptimalContrast(ImageMatrix image, int stepCount)
    {
        int width = image.Width;
        int height = image.Height;

        // 1. Площадь и диагональ
        double S = width * height;
        double D = Math.Sqrt(width * width + height * height);

        // 2. Стандартное отклонение (Контраст матрицы)
        // Вычисляем среднее
        double sum = 0;
        foreach (var p in image.Pixels) sum += p;
        double mean = sum / image.Pixels.Length;

        // Вычисляем дисперсию
        double varianceSum = 0;
        foreach (var p in image.Pixels)
        {
            double diff = p - mean;
            varianceSum += diff * diff;
        }
        double variance = varianceSum / image.Pixels.Length;
        double sigma = Math.Sqrt(variance); // Стандартное отклонение

        // 3. Формула
        // x = 3.2 * (2 * S * sigma) / (N * D)
        // 3.2 - произвольный коэффициент
        double rawX = (6.4 * S * sigma) / (stepCount * D);

        // Округляем до целого
        return (int)Math.Round(rawX);
    }

}
