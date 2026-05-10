

using Application.DTO;
using Application.Interfaces;
using Application.Repositories;
using Domain.Models;
using Domain.Services;

namespace Application.UseCases
{
    public class ImageProcessor(
        IPainter painter,
        IProgressLogger progressLogger,
        IRouteRenderer routeRenderer,
        IProcessedResultRepository resultRepository,
        RouteBuilder routeBuilder)
    {

        public async Task<IEnumerable<ProcessedResultDto>> GetRecords(CancellationToken cancellationToken = default)
        {
            return (await resultRepository.GetAllAsync(cancellationToken)).Select(r => new ProcessedResultDto(r.Id, r.Name, r.OriginalFilePath, r.ResultImagePath, r.ResultRoutePath, r.CreatedAt));
        }

        public async Task<ProcessingResponse> ProcessImageAsync(ProcessingRequest request, CancellationToken cancellationToken = default)
        {
            // Запрашиваем у сервиса хранения потоки для исходного и результатов
            using SavedRecord record = await resultRepository.AddProcessedResultAsync(
                request.FileName,
                request.OriginalStream,
                cancellationToken
            );

            // Получим данные пикселей
            PixelData[,] data = await painter.GetPixelMatrixAsync(request.OriginalStream);

            // Построим матрицы
            var originalPixelMatrix = new PixelMatrix(data);
            var routeMatrix = new RouteMatrix(originalPixelMatrix.Width, originalPixelMatrix.Height, request.CountPoints);
            await progressLogger.SendProgressAsync(Domain.Enums.ProgressStage.Loaded);
            if (routeMatrix.Points.Length == 0)
                return record.Response;

            // Найдем маршрут
            var route = new Route(routeMatrix.Points.First());
            routeBuilder.FillRoute(routeMatrix, route, originalPixelMatrix, request.ContrastLine, request.CountSteps);
            await progressLogger.SendProgressAsync(Domain.Enums.ProgressStage.Calculated);

            // Нанесем маршрут на изображение
            var resultPixelMatrix = routeRenderer.RenderRoute(route, request.Padding, originalPixelMatrix.Width, originalPixelMatrix.Height);

            // Запишем данные в результирующие потоки
            await painter.SaveImageAsync(record.ResultImage, request.Padding, routeMatrix.Points, resultPixelMatrix.Values);
            await route.WriteToStreamAsync(record.RouteFile);
            await progressLogger.SendProgressAsync(Domain.Enums.ProgressStage.Saved);

            return record.Response;
        }
    }
}
