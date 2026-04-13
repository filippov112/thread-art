

using Application.DTO;
using Application.Services;
using Domain.Models;

namespace Application.UseCases
{
    public class ImageProcessor(ISaveManager saveManager, IPainter painter, IProgressLogger progressLogger)
    {
        public async Task<ProcessingResult> ProcessImageAsync(ProcessImageRequest request)
        {
            // Сохраним изображение локально
            string originalImagePath = await saveManager.SaveOriginalImageAsync(
                request.ImageStream,
                request.Directory,
                request.FileName
            );
            await progressLogger.SendProgress(5); // Старт

            // Построим матрицы
            var sourcePixelMatrix  = await painter.GetPixelMatrix(Path.Combine(request.Directory, originalImagePath));
            var routeMatrix = new RouteMatrix(sourcePixelMatrix.Width, sourcePixelMatrix.Height, request.Config.CountPoints);
            await progressLogger.SendProgress(10); // Загрузка изображения завершена

            // Выберем стартовую точку
            var start = routeMatrix.SelectBeginPoint();

            // Создаем обертку для вызова асинхронного метода из синхронного контекста делегата
            // Вариант А: Fire-and-forget (не ждем завершения отправки прогресса, чтобы не тормозить расчет)
            void ReportProgress(int percent)
            {
                // Игнорируем ошибки отправки прогресса, чтобы не упал основной расчет
                _ = progressLogger.SendProgress(10 + (percent * 80) / 100); // Масштабируем: 10%..90%
            }


            // Найдем маршрут
            var route = await routeMatrix.BuildRoute(start, sourcePixelMatrix, request.Config.ContrastLine, request.Config.CountSteps,
                ReportProgress);
            await progressLogger.SendProgress(95); // Расчет завершен

            // Перенесем матрицу на изображение
            var newPixelMatrix = await painter.DrawImage(routeMatrix, route);

            // Сохраним изображение
            string resultImagePath = await saveManager.SaveResultImageAsync("", request.Directory, Path.GetFileName(originalImagePath));
            await painter.SaveImage(Path.Combine(request.Directory, resultImagePath), routeMatrix, newPixelMatrix);

            // Сохраним список координат маршрута
            string routeFilePath = await saveManager.SaveRouteAsync(route, request.Directory, Path.GetFileName(originalImagePath));

            await progressLogger.SendProgress(100); // Финиш
            return new ProcessingResult
            {
                OriginalImagePath = originalImagePath,
                ResultImagePath = resultImagePath,
                RouteFilePath = routeFilePath
            };
        }
    }
}
