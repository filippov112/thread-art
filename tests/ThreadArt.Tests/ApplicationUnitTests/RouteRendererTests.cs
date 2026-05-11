using Application.Services;
using Domain.Models;

namespace ThreadArt.Tests.ApplicationUnitTests;

public class RouteRendererTests
{
    [Theory]
    [InlineData(1000, 9000, 10)]
    public void RouteRenderer_Normalization(double overlaysCountDiff, int overlaysCount, double persentDiff)
    {
        var p1 = new SectorPoint(new PixelPoint(1, 0), 5, 5);
        var p2 = new SectorPoint(new PixelPoint(0, 4), 5, 5);
        var p3 = new SectorPoint(new PixelPoint(0, 3), 5, 5);
        var lineDiff = new Line(p1, p2);
        var line = new Line(p1, p3);
        Route route = new(p1);
        for (int i = 0; i < overlaysCount; i++)
            route.Lines.Add(line);
        for (int i = 0; i < overlaysCountDiff; i++)
            route.Lines.Add(lineDiff);

        var renderer = new RouteRenderer();
        var matrix = renderer.RenderRoute(route, 0, 5, 5);

        var results = ConvertMatrixToList(matrix);
        var min = results.Where(x => x > 0.1).Min();
        var max = results.Max();

        Assert.True(min >= 0);
        Assert.True(max <= 255);
        Assert.True(Math.Abs(min / max - persentDiff / 100) < 0.01);
    }

    private List<double> ConvertMatrixToList(ImageMatrix pixelMatrix)
    {
        List<double> results = [];
        for (int y = 0; y < pixelMatrix.Height - 1; y++)
            for (int x = 0; x < pixelMatrix.Width - 1; x++)
                results.Add(pixelMatrix.Pixels[y * pixelMatrix.Width + x]);
        return results;
    }
}
