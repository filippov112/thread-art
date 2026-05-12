namespace Domain.Models;

public class Route(SectorPoint start)
{
    public readonly List<SectorPoint> Points = [start,];
}
