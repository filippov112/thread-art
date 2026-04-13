using Domain.Models;

namespace Application.Services
{
    public interface ISaveManager
    {
        /// <summary>
        /// Сохраняет входящий поток и возвращает полный путь к сохраненному файлу
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<string> SaveOriginalImageAsync(Stream stream, string directory, string fileName);

        /// <summary>
        /// Сохраняет результат и возвращает путь
        /// </summary>
        /// <param name="tempPath"></param>
        /// <param name="originalFileName"></param>
        /// <returns></returns>
        Task<string> SaveResultImageAsync(string tempPath, string directory, string originalFileName);

        /// <summary>
        /// Сохраняет файл маршрута и возвращает путь сохранения
        /// </summary>
        /// <param name="route"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        Task<string> SaveRouteAsync(List<Line> route, string directory, string filename);
    }
}
