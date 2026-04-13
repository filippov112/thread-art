using Domain.Models;

namespace Application.Services
{
    public interface ISaveManager
    {
        Task<string> SaveImageAsync(Stream fileStream, Config config);
        Task SaveRouteAsync(List<string> route, string filename);
    }
}
