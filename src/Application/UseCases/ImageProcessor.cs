

using Application.DTO;
using Application.Interfaces;
using Application.Repositories;
using Domain.Models;
using Domain.Services;

namespace Application.UseCases
{
    public class ImageProcessor(
        IImageModelRepository imageRepo,
        IProcessingJobRepository jobRepo,
        IFileSystemService fileSystem)
    {
        public async Task<IEnumerable<GetRecordsDto>> GetRecords(CancellationToken ct = default)
        {
            return (await imageRepo.GetAllAsync(ct)).Select(r => new GetRecordsDto(r.Id, r.Name, r.OriginalFilePath, r.ResultImagePath, r.ResultRoutePath, r.CreatedAt));
        }

        public async Task<UploadImageDto> ProcessImageAsync(ProcessingRequest request, CancellationToken ct = default)
        {
            // Получим оригинальную матрицу
            ImageMatrix originalImage = await fileSystem.ReadOriginalImageAsync(request.SystemPath, ct);
            await jobRepo.UpdateProgressAsync(request.JobID, 5, ct);

            // Найдем координаты вершин
            var points = PointsFinder.GetPoints(originalImage.Width, originalImage.Height, request.CountPoints);
            if (points.Length == 0)
                throw new Exception("Ошибка! Недостаточное количество крайних точек!");
            await jobRepo.UpdateProgressAsync(request.JobID, 10, ct);

            // Построим маршрут
            var route = new Route(points.First());
            var start = route.Points.First();
            var contrastLine = request.ContrastLine == 0 ? RouteBuilder.CalculateOptimalContrast(originalImage, request.CountSteps) : request.ContrastLine;
            if (request.CountSteps < 7) // Цель - раздробить процесс на промежутки ~10% (до 80%)
                RouteBuilder.FillRoute(start, points, route, originalImage, request.CountSteps, contrastLine);
            else
            {
                var progressStep = request.CountSteps / 7;
                for (int i = progressStep; i <= request.CountSteps; i = Math.Clamp(i + progressStep, 0, request.CountSteps))
                {
                    var batchSize = i % progressStep == 0 ? progressStep : i % progressStep;
                    start = RouteBuilder.FillRoute(start, points, route, originalImage, batchSize, contrastLine);
                    await jobRepo.UpdateProgressAsync(request.JobID, 10 + (int)Math.Round(70d * i / request.CountSteps), ct);
                    if (i == request.CountSteps)
                        break;
                }
            }

            // Нанесем маршрут на изображение
            var resultImage = RouteRenderer.RenderRoute(route, request.Padding, originalImage.Width, originalImage.Height);

            // Запишем данные на диск
            (string resultImageSystemPath, string resultImageWebPath) = await fileSystem.SaveResultImageAsync(request.Padding, points, resultImage, ct);
            (string resultRouteSystemPath, string resultRouteWebPath) = await fileSystem.SaveRouteAsync(route, ct);

            // Сохраним запись в базе
            await imageRepo.SaveImageModelAsync(new()
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
