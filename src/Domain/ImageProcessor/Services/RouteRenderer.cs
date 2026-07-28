using Domain.ImageProcessor.Models;
namespace Domain.ImageProcessor.Services;


/// <summary>
/// Сервис отрисовки маршрута
/// </summary>
public class RouteRenderer
{
    public static int[] RenderRoute(Route route, int padding)
    {
        var w = route.Matrix.Width + padding * 2;
        var h = route.Matrix.Height + padding * 2;

        var lineMatrix = FillValues(w, h);
        for (int i = 0; i < route.Points.Count - 1; i++)
            foreach (var pixel in BresenhamAlgorithm.GetLineIterator(route.Points[i].Pixel, route.Points[i + 1].Pixel))
                lineMatrix[(pixel.Y + padding) * w + pixel.X + padding] += 1;

        (int minValue, int maxValue) = GetMinMaxValues(lineMatrix, w, h, padding);
        if (minValue == maxValue)
            minValue--;
        NormalizeValues(lineMatrix, minValue, maxValue, w, h, padding);
        return lineMatrix;
    }

    private static int[] FillValues(int w, int h)
    {
        int[] result = new int[w * h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                result[y * w + x] = 0;
        return result;
    }
    private static (int, int) GetMinMaxValues(int[] values, int w, int h, int padding)
    {
        int maxValue = 0;
        int minValue = int.MaxValue;
        // Накидываем по 1 пикселю с каждой стороны, чтобы не брать в расчет вершины
        for (int y = padding + 1; y < h - padding - 1; y++)
            for (int x = padding + 1; x < w - padding - 1; x++)
            {
                maxValue = Math.Max(maxValue, values[y * w + x]);
                minValue = Math.Min(minValue, values[y * w + x]);
            }
        return (minValue, maxValue);
    }
    private static void NormalizeValues(int[] values, int minValue, int maxValue, int w, int h, int padding)
    {
        for (int x = padding; x < w - padding; x++)
            for (int y = padding; y < h - padding; y++)
                values[y * w + x] = 255 - Math.Clamp(255 * (values[y * w + x] - minValue) / (maxValue - minValue), 0, 255);
    }
}
