using Application.DTO;
using Application.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories;

public class ProcessedResultRepository(ApplicationDbContext context, IOptions<StorageOptions> options) : IProcessedResultRepository
{
    private readonly string _webPath = options.Value.StaticFiles;
    private readonly string _storagePath = options.Value.FolderPath;

    public async Task<ProcessedResult> AddAsync(ProcessedResult entity, CancellationToken cancellationToken = default)
    {
        await context.ProcessedResults.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<SavedRecord> AddProcessedResultAsync(string fileName, Stream originalStream, CancellationToken cancellationToken = default)
    {
        string originalImageName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        string resultImageName = Guid.NewGuid().ToString() + ".png";
        string resultRouteFileName = Guid.NewGuid().ToString() + ".txt";

        Directory.CreateDirectory(Path.Combine(_webPath, _storagePath));
        var savedRecord = new SavedRecord(
            File.Create(Path.Combine(_webPath, _storagePath, resultImageName)),
            File.Create(Path.Combine(_webPath, _storagePath, resultRouteFileName)),
            new ProcessingResponse(
                "/" + string.Join("/", _storagePath, originalImageName),
                "/" + string.Join("/", _storagePath, resultImageName),
                "/" + string.Join("/", _storagePath, resultRouteFileName)
            )
        );
        await originalStream.CopyToAsync(File.Create(Path.Combine(_webPath, _storagePath, originalImageName)), cancellationToken);
        originalStream.Position = 0;

        var record = new ProcessedResult()
        {
            Name = originalImageName,
            OriginalFilePath = savedRecord.Response.OriginalImage,
            ResultImagePath = savedRecord.Response.ResultImage,
            ResultRoutePath = savedRecord.Response.ResultRoute
        };
        await AddAsync(record, cancellationToken);
        return savedRecord;
    }

    public async Task<IEnumerable<ProcessedResult>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.ProcessedResults.ToListAsync(cancellationToken);
    }
}
