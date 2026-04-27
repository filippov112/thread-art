using Application.Interfaces;
using Domain.Models;

namespace Application.Services;

public class RouteRenderer : IRouteRenderer
{
    private int _width;
    private int _height;
    private int _padding;

    public PixelMatrix RenderRoute(Route route, int padding, int width, int height)
    {
        _width = width + padding * 2;
        _height = height + padding * 2;
        _padding = padding;
        var lineMatrix = FillValues();
        foreach (var line in route.Lines)
            foreach (var point in line.Points)
                lineMatrix[point.X + padding, point.Y + padding] += 1;

        (double minValue, double maxValue) = GetMinMaxValues(lineMatrix);
        if (minValue == maxValue)
            minValue--;
        NormalizeValues(lineMatrix, minValue, maxValue);
        return new(_width, _height, lineMatrix);
    }

    private double[,] FillValues()
    {
        double[,] result = new double[_width, _height];
        for (int i = 0; i < _width; i++)
            for (int j = 0; j < _height; j++)
                result[i, j] = 0;
        return result;
    }
    private (double, double) GetMinMaxValues(double[,] values)
    {
        double maxValue = 0;
        double minValue = double.MaxValue;
        for (int i = _padding; i < _width - _padding; i++)
            for (int j = _padding; j < _height - _padding; j++)
            {
                maxValue = Math.Max(maxValue, values[i, j]);
                minValue = Math.Min(minValue, values[i, j]);
            }
        return (minValue, maxValue);
    }
    private void NormalizeValues(double[,] values, double minValue, double maxValue)
    {
        for (int i = _padding; i < _width - _padding; i++)
            for (int j = _padding; j < _height - _padding; j++)
                values[i, j] = 255 - 255 * (values[i, j] - minValue) / (maxValue - minValue);
    }
}
