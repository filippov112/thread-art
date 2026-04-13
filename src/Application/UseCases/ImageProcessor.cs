

using Application.DTO;
using Application.Services;
using Domain.Models;

namespace Application.UseCases
{
    public class ImageProcessor(ISaveManager saveManager, IPainter painter)
    {
        public async Task<ProcessingResult> ProcessImageAsync(ProcessImageRequest request)
        {
            // Сохраним изображение локально
            string originalImagePath = await saveManager.SaveOriginalImageAsync(
                request.ImageStream,
                request.Directory,
                request.FileName
            );

            // Построим матрицы
            var sourcePixelMatrix  = await painter.GetPixelMatrix(Path.Combine(request.Directory, originalImagePath));
            var routeMatrix = new RouteMatrix(sourcePixelMatrix.Width, sourcePixelMatrix.Height, request.Config.CountPoints);

            // Выберем стартовую точку
            var start = routeMatrix.SelectBeginPoint();

            // Найдем маршрут
            var route = await routeMatrix.BuildRoute(start, sourcePixelMatrix, request.Config.ContrastLine, request.Config.CountSteps);

            // Перенесем матрицу на изображение
            var newPixelMatrix = await painter.DrawImage(routeMatrix, route);

            // Сохраним изображение
            string resultImagePath = await saveManager.SaveResultImageAsync("", request.Directory, Path.GetFileName(originalImagePath));
            await painter.SaveImage(Path.Combine(request.Directory, resultImagePath), routeMatrix, newPixelMatrix);

            // Сохраним список координат маршрута
            string routeFilePath = await saveManager.SaveRouteAsync(route, request.Directory, Path.GetFileName(originalImagePath));

            return new ProcessingResult
            {
                OriginalImagePath = originalImagePath,
                ResultImagePath = resultImagePath,
                RouteFilePath = routeFilePath
            };
        }
    }
}
