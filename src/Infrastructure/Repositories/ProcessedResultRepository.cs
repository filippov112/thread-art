using Application.DTO;
using Application.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

public class ProcessedResultRepository : IProcessedResultRepository
{
    readonly ApplicationDbContext _context;
    private readonly string _webPath = "wwwroot";
    private readonly string _storagePath = "storage";
    public ProcessedResultRepository(ApplicationDbContext context, IConfigurationBuilder builder)
    {
        _context = context;

        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build().GetSection("Storage");
        if (config["FolderPath"] != null)
            _storagePath = config["FolderPath"]!;
        if (config["StaticFiles"] != null)
            _webPath = config["StaticFiles"]!;
    }
    public async Task<ProcessedResult> AddAsync(ProcessedResult entity, CancellationToken cancellationToken = default)
    {
        await _context.ProcessedResults.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
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
        return await _context.ProcessedResults.ToListAsync(cancellationToken);
    }
}
