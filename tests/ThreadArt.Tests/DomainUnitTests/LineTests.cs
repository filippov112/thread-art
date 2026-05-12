using Domain.Models;

namespace ThreadArt.Tests.DomainUnitTests;

public class LineTests
{
    [Theory]
    [InlineData(0, 0, 1, 1, 2)] // Нулевой размер и 0 точек
    public void LineLegth_Should_BeEqual(int aX, int aY, int bX, int bY, int l)
    {
        // A
        var line = LineConstructor.GetLineIterator(new PixelPoint(aX, aY), new PixelPoint(bX, bY));

        // A
        Assert.Equal(l, line.ToList().Count);
    }
}
