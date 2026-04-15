using System.ComponentModel.DataAnnotations;
using Application.DTO;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ImageController : ControllerBase
{
    private readonly ImageProcessor _imageService;
    private readonly IWebHostEnvironment _env;
    private readonly IStreamController _streamController;
    private readonly IPathManager _pathManager;

    public ImageController(ImageProcessor imageService, IWebHostEnvironment env, IStreamController streamController, IPathManager pathManager)
    {
        _imageService = imageService;
        _env = env;
        _streamController = streamController;
        _pathManager = pathManager;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(ResultDTO), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ResultDTO>> UploadImage(
        [Required] IFormFile imageFile,
        [Range(1, 2000)] int countPoints = 240,
        [Range(1, 50000)] int countSteps = 4000,
        [Range(0.1, 100.0)] double contrastLine = 15)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return BadRequest("Файл изображения не предоставлен.");
        }

        _pathManager.InitNamesAndPaths(_env.WebRootPath, imageFile.FileName);

        // Объявляем переменные потоков
        Stream? originalStream = null;
        Stream? resultImageStream = null;
        Stream? resultRouteStream = null;

        try
        {
            // 1. Копируем входящий файл в память
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Сохраняем исходник на диск
            await _streamController.SaveFromMemory(_pathManager.OriginalImagePath, memoryStream);

            // 2. Создаем выходные потоки
            originalStream = memoryStream;
            resultImageStream = _streamController.MakeStream(_pathManager.ResultImagePath);
            resultRouteStream = _streamController.MakeStream(_pathManager.ResultRouteFilePath);

            // 3. Формируем запрос и запускаем процессор
            var request = new ProcessingRequest
            {
                OriginalImageStream = originalStream,
                ResultImageStream = resultImageStream,
                ResultRouteStream = resultRouteStream,
                CountPoints = countPoints,
                CountSteps = countSteps,
                ContrastLine = contrastLine
            };
            await _imageService.ProcessImageAsync(request);

            resultImageStream?.Close();
            resultRouteStream?.Close();

            var viewModel = new ResultDTO
            {
                OriginalImagePath = _pathManager.OriginalImagePathVM,
                ResultImagePath = _pathManager.ResultImagePathVM,
                ResultRoutePath = _pathManager.ResultRouteFilePathVM,
            };

            return Ok(viewModel);
        }
        catch (Exception ex)
        {
            //ModelState.AddModelError("", "Ошибка обработки изображения: " + ex.Message);
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
        finally
        {
            originalStream?.Dispose();
            resultImageStream?.Dispose();
            resultRouteStream?.Dispose();
        }
    }

    [HttpGet("info")]
    [ProducesResponseType(typeof(string), 200)]
    public ActionResult<string> GetInfo()
    {
        return Ok("API для обработки изображений Thread Art");
    }
}
