using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public interface ISaveManager
    {
        Task<string> SaveImageAsync(IFormFile file, Config config);
        Task SaveRouteAsync(List<string> route, string filename);
    }
}
