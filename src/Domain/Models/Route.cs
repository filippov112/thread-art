using System.Text;
namespace Domain.Models;

public class Route(SectorPoint start)
{
    public readonly List<SectorPoint> Points = [start, ];

    public async Task WriteToStreamAsync(Stream routeStream)
    {
        List<string> steps = [.. Points.Select(p => p.ToString())];
        byte[] buffer = Encoding.UTF8.GetBytes(string.Join('\n', steps));
        await routeStream.WriteAsync(buffer);
    }
}
