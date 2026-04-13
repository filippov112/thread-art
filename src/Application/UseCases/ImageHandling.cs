

using Application.Services;
using Domain.Models;

namespace Application.UseCases
{
    public class ImageHandling(ISaveManager saveManager, IPainter painter)
    {
        public async Task ProcessImage(Stream fileStream, Config config)
        {
            // Сохраним изображение локально
            await saveManager.SaveImageAsync(fileStream, config);

            // Построим матрицы
            var pixelMatrix  = await painter.GetPixelMatrix(config.OriginalImagePath);
            var routeMatrix = new RouteMatrix(pixelMatrix.Width, pixelMatrix.Height, config.CountPoints);

            // Выберем стартовую точку
            var start = routeMatrix.SelectBeginPoint();

            // Найдем маршрут
            var route = await routeMatrix.BuildRoute(start, pixelMatrix, config.ContrastLine, config.CountSteps);

            // Перенесем матрицу на изображение
            await painter.DrawImage(routeMatrix, route);

            // Нанесем координатную сетку
            painter.DrawCoordinateGrid(routeMatrix);

            // Сохраним изображение
            await painter.SaveImage(config.ResultImagePath);

            // Сохраним список координат маршрута
            await saveManager.SaveRouteAsync(route, config.ResultRoutePath);
        }
    }
}
