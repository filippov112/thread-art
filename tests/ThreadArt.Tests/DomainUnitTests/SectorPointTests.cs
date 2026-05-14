using Domain.Models;

namespace ThreadArt.Tests.DomainUnitTests;

public class SectorPointTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0, true)]
    [InlineData(0, 0, 0, 0, 100, 100, true)]
    [InlineData(1, 10, 1, 10, 100, 100, true)]
    [InlineData(1, 10, 0, 0, 100, 100, false)]
    [InlineData(101, 100, 101, 100, 10, 10, true)]
    public void SectorPoint_Should_BeEqual(int ax, int ay, int bx, int by, int width, int height, bool expected)
    {
        Assert.Equal(expected, new SectorPoint(new(ax, ay), width, height, 1) == new SectorPoint(new(bx, by), width, height, 1));
        Assert.Equal(!expected, new SectorPoint(new(ax, ay), width, height, 1) != new SectorPoint(new(bx, by), width, height, 1));
        Assert.Equal(expected, new SectorPoint(new(ax, ay), width, height, 1).Equals(new SectorPoint(new(bx, by), width, height, 1)));
        Assert.Equal(expected, new SectorPoint(new(ax, ay), width, height, 1).GetHashCode() == new SectorPoint(new(bx, by), width, height, 1).GetHashCode());
        Assert.Equal(expected, new SectorPoint(new(ax, ay), width, height, 1).ToString() == new SectorPoint(new(bx, by), width, height, 1).ToString());
    }

    [Theory]
    [InlineData(1, 0, 10, 20, 'T')] // y == 0
    [InlineData(1, 19, 10, 20, 'B')] // y == height - 1
    [InlineData(0, 1, 10, 20, 'L')] // x == 0
    [InlineData(9, 1, 10, 20, 'R')] // x == width - 1
    public void SectorPoint_Sector_Should_Be(int x, int y, int width, int height, char expected)
    {
        Assert.Equal(expected, new SectorPoint(new(x, y), width, height, 1).Sector);
    }
}
