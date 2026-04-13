using Application.Services;
using Domain.Models;

namespace Infrastructure
{
    public class SaveManager : ISaveManager
    {
        public async Task SaveImageAsync(Stream fileStream, Config config)
        {

            var newName = $"{Guid.NewGuid()}{config.Extension}";
            var filePath = Path.Combine(config.WebRootPath, newName);
            using var stream = new FileStream(filePath, FileMode.Create);

            await fileStream.CopyToAsync(stream);
            config.OriginalImagePath = filePath;
        }

        public async Task SaveRouteAsync(List<Line> route, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                if (route.Count > 0)
                    await writer.WriteLineAsync(route[0].Start.ToString());
                foreach (var line in route)
                {
                    await writer.WriteLineAsync(line.End.ToString());
                }
            }
        }
    }
}
