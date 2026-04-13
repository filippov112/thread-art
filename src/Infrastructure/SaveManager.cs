using Application.Services;
using Domain.Models;

namespace Infrastructure
{
    public class SaveManager : ISaveManager
    {
        public async Task<string> SaveImageAsync(Stream fileStream, Config config)
        {

            var newName = $"{Guid.NewGuid()}{config.Extension}";
            var filePath = Path.Combine(config.WebRootPath, newName);
            using var stream = new FileStream(filePath, FileMode.Create);

            await fileStream.CopyToAsync(stream);
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
