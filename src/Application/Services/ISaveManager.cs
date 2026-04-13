using Domain.Models;

namespace Application.Services
{
    public interface ISaveManager
    {
        Task SaveImageAsync(Stream fileStream, Config config);
        Task SaveRouteAsync(List<Line> route, string filename);
    }
}
