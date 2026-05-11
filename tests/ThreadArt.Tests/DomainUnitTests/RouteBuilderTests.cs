using Domain.Models;
using Domain.Services;

namespace ThreadArt.Tests.DomainUnitTests;

public class RouteBuilderTests
{
    [Theory]
    [InlineData(5, 5, 12, 0.15, 3, 1)]
    public void RouteBuilder_StepCount(int width, int height, int n, double contrast, int countSteps, byte fillValue)
    {
        var routeMatrix = new RouteMatrix(width, height, n);
        var pixelMatrix = GetMatrix(width, height, fillValue);
        var route = GetRoute(width, height, n, contrast, countSteps, routeMatrix, pixelMatrix);

        Assert.Equal(countSteps, route.Lines.Count);
    }

    [Theory]
    [InlineData(40, 50, 12, 0.15, 3, 5, 1, 1.2)] // Цель поднять значение 1 за 3 хода по 0,15 (по факту за 2, т.к. возвращаться нельзя) до >1,2
    [InlineData(20, 20, 12, 0.15, 5, 5, 1, 1.4)] // Цель поднять значение 1 за 5 хода по 0,15 (по факту за 3, т.к. возвращаться нельзя) до >1,4
    public void RouteBuilder_SelectingPath(int width, int height, int n, double contrast, int countSteps, byte fillValue, byte minValue, byte biggerThan)
    {
        var routeMatrix = new RouteMatrix(width, height, n);
        var pixelMatrix = GetMatrix(width, height, fillValue);
        pixelMatrix.Values[7, 10] = minValue; // Выбираем случайную точку матрицы (7:10)
        GetRoute(width, height, n, contrast, countSteps, routeMatrix, pixelMatrix);

        List<double> results = ConvertMatrixToList(pixelMatrix);

        var min = results.Min();
        Assert.True(min > biggerThan);
        Assert.True(min < fillValue);
    }

    [Theory]
    [InlineData(40, 50, 60, 0.04, 300, 5, 1)]
    [InlineData(20, 20, 40, 0.04, 500, 5, 1)]
    [InlineData(200, 200, 200, 0.04, 500, 5, 1)]
    public void RouteBuilder_DecreaseMinMaxDifference(int width, int height, int n, double contrast, int countSteps, byte fillValue, byte minValue)
    {
        var routeMatrix = new RouteMatrix(width, height, n);
        var pixelMatrix = GetMatrix(width, height, fillValue);
        pixelMatrix.Values[7, 10] = minValue; // Выбираем случайную точку матрицы (7:10)
        GetRoute(width, height, n, contrast, countSteps, routeMatrix, pixelMatrix);

        List<double> results = ConvertMatrixToList(pixelMatrix);

        var min = results.Min();
        var max = results.Max();
        Assert.True(max - min < fillValue - minValue);
    }

    private static PixelMatrix GetMatrix(int width, int height, byte fillValue)
    {
        PixelData[,] data = new PixelData[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                data[i, j] = new PixelData(fillValue, fillValue, fillValue);
            }
        return new PixelMatrix(data);
    }

    private static Route GetRoute(int width, int height, int n, double contrast, int countSteps, RouteMatrix routeMatrix, PixelMatrix originalPixelMatrix)
    {
        var route = new Route(routeMatrix.Points.First());
        var routeBuilder = new RouteBuilder();
        RouteBuilder.FillRoute(routeMatrix, route, originalPixelMatrix, contrast, countSteps);
        return route;
    }

    private List<double> ConvertMatrixToList(PixelMatrix pixelMatrix)
    {
        List<double> results = [];
        for (int i = 0; i < pixelMatrix.Values.GetLength(0) - 1; i++)
            for (int j = 0; j < pixelMatrix.Values.GetLength(1) - 1; j++)
                results.Add(pixelMatrix.Values[i, j]);
        return results;
    }
}
