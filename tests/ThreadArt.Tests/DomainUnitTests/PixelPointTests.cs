using Domain.Models;

namespace ThreadArt.Tests.DomainUnitTests;

public class PixelPointTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, true)]
    [InlineData(1, 10, 1, 10, true)]
    [InlineData(1, 10, 0, 0, false)]
    [InlineData(101, 100, 101, 100, true)]
    public void PixelPoint_Should_BeEqual(int ax, int ay, int bx, int by, bool expected)
    {
        Assert.Equal(expected, new PixelPoint(ax, ay) == new PixelPoint(bx, by));
        Assert.Equal(!expected, new PixelPoint(ax, ay) != new PixelPoint(bx, by));
        Assert.Equal(expected, new PixelPoint(ax, ay).Equals(new PixelPoint(bx, by)));
        Assert.Equal(expected, new PixelPoint(ax, ay).GetHashCode() == new PixelPoint(bx, by).GetHashCode());
    }
}
