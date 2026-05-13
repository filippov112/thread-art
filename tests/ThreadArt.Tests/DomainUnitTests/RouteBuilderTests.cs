using Domain.Models;
using Domain.Services;

namespace ThreadArt.Tests.DomainUnitTests;

public class RouteBuilderTests
{
    [Theory]
    [InlineData(5, 5, 12, 0.15, 3, 1)]
    public void RouteBuilder_StepCount(int width, int height, int n, int contrast, int countSteps, int fillValue)
    {
        var routeMatrix = PointsFinder.GetPoints(width, height, n);
        var pixelMatrix = GetMatrix(width, height, fillValue);
        var route = GetRoute(contrast, countSteps, routeMatrix, pixelMatrix);

        Assert.Equal(countSteps + 1, route.Points.Count);
    }

    [Theory]
    [InlineData(40, 50, 36, 7, 1, 500, 100, 106)] // Цель поднять значение 100 за 3 хода по 7 (по факту за 2, т.к. возвращаться нельзя) до >106
    [InlineData(20, 20, 24, 7, 3, 500, 100, 112)] // Цель поднять значение 100 за 5 хода по 7 (по факту за 3, т.к. возвращаться нельзя) до >112
    public void RouteBuilder_SelectingPath(int width, int height, int n, int contrast, int countSteps, int fillValue, int minValue, int biggerThan)
    {
        var routeMatrix = PointsFinder.GetPoints(width, height, n);
        var pixelMatrix = GetMatrix(width, height, fillValue);
        pixelMatrix.Pixels[10 * width + 7] = minValue; // Выбираем случайную точку матрицы (7:10)
        GetRoute(contrast, countSteps, routeMatrix, pixelMatrix);


        var min = pixelMatrix.Pixels.Min();
        Assert.True(min > biggerThan);
        Assert.True(min < fillValue);
    }

    [Theory]
    [InlineData(40, 50, 60, 4, 600, 500, 100)]
    [InlineData(20, 20, 40, 4, 1000, 500, 100)]
    [InlineData(200, 200, 200, 4, 1000, 500, 100)]
    public void RouteBuilder_DecreaseMinMaxDifference(int width, int height, int n, int contrast, int countSteps, int fillValue, int minValue)
    {
        var routeMatrix = PointsFinder.GetPoints(width, height, n);
        var pixelMatrix = GetMatrix(width, height, fillValue);
        pixelMatrix.Pixels[10 * width + 7] = minValue; // Выбираем случайную точку матрицы (7:10)
        GetRoute(contrast, countSteps, routeMatrix, pixelMatrix);

        var min = pixelMatrix.Pixels.Min();
        var max = pixelMatrix.Pixels.Max();
        Assert.True(max - min < fillValue - minValue);
    }

    private static ImageMatrix GetMatrix(int width, int height, int fillValue)
    {
        int[] data = new int[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                data[y * width + x] = fillValue;
            }
        return new(width, height, data);
    }

    private static Route GetRoute(int contrast, int countSteps, SectorPoint[] routeMatrix, ImageMatrix originalPixelMatrix)
    {
        var route = new Route(routeMatrix.First());
        RouteBuilder.FillRoute(routeMatrix.First(), routeMatrix, route, originalPixelMatrix, contrast, countSteps);
        return route;
    }
}
