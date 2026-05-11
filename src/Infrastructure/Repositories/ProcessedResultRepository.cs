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

    public async Task<SavedRecord> AddProcessedResultAsync(string fileName, Stream originalStream, CancellationToken cancellationToken = default)
    {
        string originalImageName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        string resultImageName = Guid.NewGuid().ToString() + ".png";
        string resultRouteFileName = Guid.NewGuid().ToString() + ".txt";

        Directory.CreateDirectory(Path.Combine(_webPath, _storagePath));
        var savedRecord = new SavedRecord(
            File.Create(Path.Combine(_webPath, _storagePath, resultImageName)),
            File.Create(Path.Combine(_webPath, _storagePath, resultRouteFileName)),
            new UploadImageDto(
                "/" + string.Join("/", _storagePath, originalImageName),
                "/" + string.Join("/", _storagePath, resultImageName),
                "/" + string.Join("/", _storagePath, resultRouteFileName)
            )
        );
        await originalStream.CopyToAsync(File.Create(Path.Combine(_webPath, _storagePath, originalImageName)), cancellationToken);
        originalStream.Position = 0;

        var record = new ImageModel()
        {
            Name = originalImageName,
            OriginalFilePath = savedRecord.Response.OriginalImagePath,
            ResultImagePath = savedRecord.Response.ResultImagePath,
            ResultRoutePath = savedRecord.Response.ResultRoutePath
        };
        await context.Images.AddAsync(record, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return savedRecord;
    }

    public async Task<IEnumerable<ImageModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Images.ToListAsync(cancellationToken);
    }
}
