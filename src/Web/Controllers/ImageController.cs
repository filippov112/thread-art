using System.ComponentModel.DataAnnotations;
using Application.DTO;
using Application.UseCases;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ImageController(IServiceScopeFactory scopeFactory) : ControllerBase
{
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
        using var scope = scopeFactory.CreateScope();
        var imageService = scope.ServiceProvider.GetRequiredService<ImageProcessor>();

        // Объявляем поток
        Stream? originalStream = null;

        try
        {
            // Копируем входящий файл в память
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            originalStream = memoryStream;

            // Формируем запрос и запускаем процессор
            var request = new ProcessingRequest
            {
                FileName = imageFile.FileName,
                OriginalStream = originalStream,
                CountPoints = countPoints,
                CountSteps = countSteps,
                ContrastLine = contrastLine
            };
            var response = await imageService.ProcessImageAsync(request);

            var viewModel = new ResultDTO
            {
                OriginalImagePath = response.OriginalImage,
                ResultImagePath = response.ResultImage,
                ResultRoutePath = response.ResultRoute,
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
            GC.Collect();
        }
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<ProcessedResultDto>), 200)]
    public async Task<ActionResult<IEnumerable<ProcessedResultDto>>> GetRecords()
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var imageService = scope.ServiceProvider.GetRequiredService<ImageProcessor>();
            var result = await imageService.GetRecords();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }
}
