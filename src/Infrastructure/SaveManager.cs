using Application.Services;
using Domain.Models;

namespace Infrastructure
{
    public class SaveManager : ISaveManager
    {

        public async Task<string> SaveOriginalImageAsync(Stream stream, string directory, string fileName)
        {
            Directory.CreateDirectory(Path.Combine(directory, "images", "input"));
            var uniqueName = Path.Combine("images", "input", $"{Guid.NewGuid()}{Path.GetExtension(fileName)}");

            using (var fileStream = new FileStream(Path.Combine(directory, uniqueName), FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return uniqueName;
        }

        public async Task<string> SaveResultImageAsync(string tempPath, string directory, string originalFileName)
        {
            Directory.CreateDirectory(Path.Combine(directory, "images", "result"));
            var uniqueName = Path.Combine("images", "result", $"{Guid.NewGuid()}_result.png");
            return uniqueName;
        }

        public async Task<string> SaveRouteAsync(List<Line> route, string directory, string filename)
        {
            Directory.CreateDirectory(Path.Combine(directory, "images", "routes"));
            var uniqueName = Path.Combine("images", "routes", $"{Guid.NewGuid()}_route.txt");
            if (route.Count == 0)
            {
                await File.WriteAllLinesAsync(Path.Combine(directory, uniqueName), []);
                return uniqueName;
            }
            List<string> points = [route[0].Start.ToString(), .. route.Select(l => l.End.ToString())];
            await File.WriteAllLinesAsync(Path.Combine(directory, uniqueName), points);
            return uniqueName;
        }
    }
}
