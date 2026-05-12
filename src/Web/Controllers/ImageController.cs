using System.ComponentModel.DataAnnotations;
using Application.DTO;
using Application.Interfaces;
using Application.UseCases;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ImageController(IServiceScopeFactory scopeFactory, IFileSystemService fileSystem) : ControllerBase
{
    [HttpPost("upload")]
    [ProducesResponseType(typeof(UploadImageDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UploadImage(
        [Required] IFormFile imageFile,
        [Range(1, 2000)] int countPoints = 240,
        [Range(1, 50000)] int countSteps = 4000,
        [Range(0.1, 100.0)] int contrastLine = 15,
        [Range(1, 300)] int padding = 10
    )
    {
        if (imageFile == null || imageFile.Length == 0)
            return BadRequest("Файл не предоставлен.");
        try
        {
            using var scope = scopeFactory.CreateScope();

            // Сохраняем файл
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            (string systemPath, string webPath) = await fileSystem.SaveOriginalImageAsync(imageFile.FileName, memoryStream);

            // Создаём задачу в БД
            var job = new ProcessingJob
            {
                FileName = imageFile.FileName,
                OriginalSystemPath = systemPath,
                OriginalWebPath = webPath,
                CountPoints = countPoints,
                CountSteps = countSteps,
                ContrastLine = contrastLine,
                Padding = padding
            };
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Jobs.AddAsync(job);
            await db.SaveChangesAsync();

            // Ставим в очередь
            var queue = scope.ServiceProvider.GetRequiredService<IJobQueue>();
            await queue.EnqueueAsync(job.Id);

            return Accepted(new { JobId = job.Id, StatusUrl = $"/api/image/job/{job.Id}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }

    [HttpGet("job/{jobId}")]
    public async Task<IActionResult> GetJobStatus(Guid jobId)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var job = await db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        return job == null ? NotFound() : Ok(job);
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetAllJobStatus()
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var jobs = db.Jobs.ToArray();
        return jobs == null ? NotFound() : Ok(jobs);
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<GetRecordsDto>), 200)]
    public async Task<ActionResult<IEnumerable<GetRecordsDto>>> GetRecords()
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
