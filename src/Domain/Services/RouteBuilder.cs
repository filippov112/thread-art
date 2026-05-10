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
    public void FillRoute(RouteMatrix matrix, Route route, PixelMatrix originalImage, double lineContrast, int stepCount)
    {
        var start = route.Start;
        for (int step = 0; step < stepCount; step++)
        {
            var line = FindNextLine(matrix, start, route, originalImage);
            if (line != null)
            {
                route.Lines.Add(line);
                foreach (var p in line.Points)
                {
                    originalImage.Values[p.X, p.Y] += lineContrast;
                }
                start = line.End;
            }
        }
    }

    private Line? FindNextLine(RouteMatrix matrix, SectorPoint start, Route route, PixelMatrix originalImage)
    {
        double minValue = double.MaxValue;
        Line? bestPath = matrix.Paths[start].FirstOrDefault();
        if (bestPath == null)
            return bestPath;
        foreach (var path in matrix.Paths[start])
        {
            double sum = 0;
            int count = 0;

            foreach (var p in path.Points)
            {
                sum += originalImage.Values[p.X, p.Y];
                count++;
            }

            if (count > 0)
            {
                double avgProb = sum / count;
                // Запрещаем возвращаться по тому же маршруту
                if (route.Lines.Count > 0 && route.Lines.Last().IsRevert(path))
                    continue;
                if (avgProb < minValue)
                {
                    minValue = avgProb;
                    bestPath = path;
                }
            }
        }
        return bestPath;
    }

}
