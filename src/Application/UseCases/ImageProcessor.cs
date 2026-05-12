

using Application.DTO;
using Application.Interfaces;
using Application.Repositories;
using Domain.Models;
using Domain.Services;

namespace Application.UseCases
{
    public class ImageProcessor(
        IRouteRenderer routeRenderer,
        IImageModelRepository repository,
        IFileSystemService fileSystem)
    {

        public async Task<IEnumerable<GetRecordsDto>> GetRecords(CancellationToken ct = default)
        {
            return (await repository.GetAllAsync(ct)).Select(r => new GetRecordsDto(r.Id, r.Name, r.OriginalFilePath, r.ResultImagePath, r.ResultRoutePath, r.CreatedAt));
        }

        public async Task<UploadImageDto> ProcessImageAsync(ProcessingRequest request, CancellationToken ct = default)
        {
            // Получим оригинальную матрицу
            ImageMatrix originalImage = await fileSystem.ReadOriginalImageAsync(request.SystemPath, ct);

            // Найдем координаты вершин
            var points = PointsFinder.GetPoints(originalImage.Width, originalImage.Height, request.CountPoints);
            if (points.Length == 0)
                throw new Exception("Ошибка! Недостаточное количество крайних точек!");

            // Построим маршрут
            var route = new Route(points.First());
            RouteBuilder.FillRoute(points, route, originalImage, request.ContrastLine, request.CountSteps);

            // Нанесем маршрут на изображение
            var resultImage = routeRenderer.RenderRoute(route, request.Padding, originalImage.Width, originalImage.Height);

            // Запишем данные на диск
            (string resultImageSystemPath, string resultImageWebPath) = await fileSystem.SaveResultImageAsync(request.Padding, points, resultImage, ct);
            (string resultRouteSystemPath, string resultRouteWebPath) = await fileSystem.SaveRouteAsync(route, ct);

            // Сохраним запись в базе
            await repository.SaveImageModelAsync(new()
            {
                Name = request.FileName,
                OriginalFilePath = request.SystemPath,
                ResultImagePath = resultImageSystemPath,
                ResultRoutePath = resultRouteSystemPath
            }, ct);

            return new(resultImageWebPath, resultRouteWebPath);
        }
    }
}
