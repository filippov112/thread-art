using System.Text;
namespace Domain.Models;

public class Route(SectorPoint start)
{
    public readonly SectorPoint Start = start;
    public readonly List<Line> Lines = [];

    public async Task WriteToStreamAsync(Stream routeStream)
    {
        List<string> steps = [Start.ToString(), .. Lines.Select(l => l.End.ToString())];
        byte[] buffer = Encoding.UTF8.GetBytes(string.Join('\n', steps));
        await routeStream.WriteAsync(buffer);
    }
}
