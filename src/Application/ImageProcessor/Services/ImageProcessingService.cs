using Application.ImageProcessor.DTO;
using Application.QueueManager.Repositories;
using Domain.ImageProcessor.Models;
using Domain.ImageProcessor.Services;
using Domain.QueueManager.Services;

namespace Application.ImageProcessor.Services;



public class ImageProcessingService(IProcessingJobRepository jobRepo, IFileSystemService fileSystem)
{
    public async Task<ResponseDto> ProcessImageAsync(RequestDto request, CancellationToken ct = default)
    {
        // Получим матрицу
        ImageMatrix matrix = await fileSystem.ReadOriginalImageAsync(request.SystemPath, request.CountPoints, ct);
        await jobRepo.UpdateProgressAsync(request.JobID, 10, ct);

        // Построим маршрут
        ProgressLogger logger = new();
        logger.ProgressUpdated += async val => await UpdateProgress(request.JobID, val, ct);
        var route = new Route(request.CountSteps, matrix, logger);

        // Нанесем маршрут на изображение
        int[] resultImageData = RouteRenderer.RenderRoute(route, request.Padding);

        // Запишем данные на диск
        string resultImageSystemPath = await fileSystem.SaveResultImageAsync(
            innerWidth: matrix.Width,
            innerHeight: matrix.Height,
            padding: request.Padding,
            data: resultImageData,
            sidePoints: matrix.SidePoints!,
            ct: ct
            );
        string resultRouteSystemPath = await fileSystem.SaveRouteAsync(route, ct);

        return new(resultImageSystemPath, resultRouteSystemPath);
    }

    private async Task UpdateProgress(Guid jobId, int val, CancellationToken ct)
    {
        await jobRepo.UpdateProgressAsync(jobId, val, ct);
    }
}

