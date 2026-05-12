using Application.Interfaces;
using Domain.Models;

namespace Application.Services;

public class RouteRenderer : IRouteRenderer
{
    private int _width;
    private int _height;
    private int _padding;

    public ImageMatrix RenderRoute(Route route, int padding, int width, int height)
    {
        _width = width + padding * 2;
        _height = height + padding * 2;
        _padding = padding;
        var lineMatrix = FillValues();
        for (int i = 0; i < route.Points.Count - 1; i++)
            foreach (var pixel in LineConstructor.GetLineIterator(route.Points[i].Pixel, route.Points[i + 1].Pixel))
                lineMatrix[(pixel.Y + padding) * _width + pixel.X + padding] += 1;

        (int minValue, int maxValue) = GetMinMaxValues(lineMatrix);
        if (minValue == maxValue)
            minValue--;
        NormalizeValues(lineMatrix, minValue, maxValue);
        return new(_width, _height, lineMatrix);
    }

    private int[] FillValues()
    {
        int[] result = new int[_width * _height];
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                result[y * _width + x] = 0;
        return result;
    }
    private (int, int) GetMinMaxValues(int[] values)
    {
        int maxValue = 0;
        int minValue = int.MaxValue;
        for (int y = _padding; y < _height - _padding; y++)
            for (int x = _padding; x < _width - _padding; x++)
            {
                maxValue = Math.Max(maxValue, values[y * _width + x]);
                minValue = Math.Min(minValue, values[y * _width + x]);
            }
        return (minValue, maxValue);
    }
    private void NormalizeValues(int[] values, int minValue, int maxValue)
    {
        for (int x = _padding; x < _width - _padding; x++)
            for (int y = _padding; y < _height - _padding; y++)
                values[y * _width + x] = 255 - 255 * (values[y * _width + x] - minValue) / (maxValue - minValue);
    }
}
