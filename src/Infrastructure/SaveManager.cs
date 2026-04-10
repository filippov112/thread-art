using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class SaveManager : ISaveManager
    {
        public async Task<string> SaveImageAsync(IFormFile file, Config config)
        {

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(config.OriginalImagePath);
            var filePath = Path.Combine(config.WebRootPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task SaveRouteAsync(List<string> route, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                foreach (var point in route)
                {
                    await writer.WriteLineAsync(point);
                }
            }
        }
    }
}