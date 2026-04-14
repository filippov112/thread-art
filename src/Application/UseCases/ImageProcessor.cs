

using Application.DTO;
using Application.Interfaces;
using Domain.Models;
using Domain.Services;

namespace Application.UseCases
{
    public class ImageProcessor(IPainter painter, IProgressLogger progressLogger, IRouteRenderer routeRenderer)
    {
        public async Task ProcessImageAsync(ProcessingRequest request)
        {
            // Получим данные пикселей
            PixelData[,] data = await painter.GetPixelMatrixAsync(request.OriginalImageStream);

            // Построим матрицы
            var originalPixelMatrix = new PixelMatrix(data);
            var routeMatrix = new RouteMatrix(originalPixelMatrix.Width, originalPixelMatrix.Height, request.CountPoints);
            await progressLogger.SendProgressAsync(Domain.Enums.ProgressStage.Loaded);
            if (routeMatrix.Points.Length == 0)
                return;

            // Найдем маршрут
            var route = new Route(routeMatrix.Points.First());
            var routeBuilder = new RouteBuilder(routeMatrix);
            routeBuilder.FillRoute(route, originalPixelMatrix, request.ContrastLine, request.CountSteps);
            await progressLogger.SendProgressAsync(Domain.Enums.ProgressStage.Calculated);

            // Нанесем маршрут на изображение
            var resultPixelMatrix = routeRenderer.RenderRoute(route, request.Padding, originalPixelMatrix.Width, originalPixelMatrix.Height);

            // Запишем данные в результирующие потоки
            await painter.SaveImageAsync(request.ResultImageStream, request.Padding, routeMatrix.Points, resultPixelMatrix.Values);
            await route.WriteToStreamAsync(request.ResultRouteStream);
            await progressLogger.SendProgressAsync(Domain.Enums.ProgressStage.Saved);
        }
    }
}
