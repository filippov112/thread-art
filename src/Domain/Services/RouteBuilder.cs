using System.Drawing;
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
    public static void FillRoute(RouteMatrix matrix, Route route, ImageMatrix originalImage, int lineContrast, int stepCount)
    {
        var start = route.Points.First();
        for (int step = 0; step < stepCount; step++)
        {
            var point = FindNextPoint(matrix, start, route, originalImage, lineContrast);
            if (point == null)
                continue;
            start = (SectorPoint)point;
            route.Points.Add(start);
        }
    }

    private static SectorPoint? FindNextPoint(RouteMatrix matrix, SectorPoint start, Route route, ImageMatrix originalImage, int lineContrast)
    {
        double minValue = int.MaxValue;
        var ends = matrix.GetLineEndPoints(start);
        SectorPoint? bestEndPoint = ends.FirstOrDefault();
        if (bestEndPoint == null)
            return null;
        foreach (var end in ends)
        {
            double sum = 0;
            int count = 0;
            foreach (var p in Line.GetLineIterator(start.Pixel, end.Pixel))
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
        foreach (var p in Line.GetLineIterator(start.Pixel, ((SectorPoint)bestEndPoint).Pixel))
        {
            originalImage.Pixels[p.Y * originalImage.Width + p.X] += lineContrast;
        }
        return bestEndPoint;
    }

}
