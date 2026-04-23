using Domain.Models;

namespace ThreadArt.Tests.DomainUnitTests;

public class LineTests
{
    [Theory]
    [InlineData(0, 0, 1, 1, 2)] // Нулевой размер и 0 точек
    public void LineLegth_Should_BeEqual(int aX, int aY, int bX, int bY, int l)
    {
        // A
        var line = new Line(new SectorPoint(new PixelPoint(aX, aY), 0, 0), new SectorPoint(new PixelPoint(bX, bY), 0, 0));

        // A
        Assert.Equal(l, line.Points.Count);
    }

    [Theory]
    [InlineData(0, 0, 5, 5, 5, 5, 0, 0)]
    public void Line_Should_IsRevert(int aX, int aY, int bX, int bY, int cX, int cY, int dX, int dY)
    {
        // A
        var lineA = new Line(new SectorPoint(new PixelPoint(aX, aY), 0, 0), new SectorPoint(new PixelPoint(bX, bY), 0, 0));
        var lineB = new Line(new SectorPoint(new PixelPoint(cX, cY), 0, 0), new SectorPoint(new PixelPoint(dX, dY), 0, 0));

        // A
        Assert.True(lineA.IsRevert(lineB));
    }

    [Theory]
    [InlineData(0, 0, 5, 5, 5, 5, 0, 1)]
    public void Line_Should_IsNotRevert(int aX, int aY, int bX, int bY, int cX, int cY, int dX, int dY)
    {
        // A
        var lineA = new Line(new SectorPoint(new PixelPoint(aX, aY), 0, 0), new SectorPoint(new PixelPoint(bX, bY), 0, 0));
        var lineB = new Line(new SectorPoint(new PixelPoint(cX, cY), 0, 0), new SectorPoint(new PixelPoint(dX, dY), 0, 0));

        // A
        Assert.False(lineA.IsRevert(lineB));
    }
}
