using Domain.Models;
using Domain.Services;

namespace ThreadArt.Tests.ApplicationUnitTests;

public class RouteRendererTests
{
    [Theory]
    [InlineData(1000, 9000, 5)]
    public void RouteRenderer_Normalization(double overlaysCountDiff, int overlaysCount, double persentDiff)
    {
        var p1 = new SectorPoint(new PixelPoint(1, 0), 5, 5, 1);
        var p2 = new SectorPoint(new PixelPoint(0, 4), 5, 5, 2);
        var p3 = new SectorPoint(new PixelPoint(0, 3), 5, 5, 3);

        Route route = new(p1);
        for (int i = 0; i < overlaysCount; i++)
        {
            route.Points.Add(p3);
            route.Points.Add(p1);
        }
        for (int i = 0; i < overlaysCountDiff; i++)
        {
            route.Points.Add(p2);
            route.Points.Add(p1);
        }

        var renderer = new RouteRenderer();
        var matrix = RouteRenderer.RenderRoute(route, 0, 5, 5);

        double min = matrix.Pixels.Where(x => x > 0.1).Min();
        double max = matrix.Pixels.Max();

        Assert.True(min >= 0);
        Assert.True(max <= 255);
        Assert.True(Math.Abs(min / max - persentDiff / 100) < 0.01);
    }
}
