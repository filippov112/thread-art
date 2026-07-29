using Core.QueueManager.DTO;
using Core.QueueManager.Models;
using Core.QueueManager.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueueManager.Repositories;

public class ProcessingJobRepository(ApplicationDbContext context) : IProcessingJobRepository
{
    public async Task UpdateProgressAsync(Guid jobId, int progress, CancellationToken ct = default)
    {
        var job = await context.Jobs.FindAsync([jobId], ct);
        if (job != null)
        {
            // Защита от выхода за пределы 0-100
            job.Progress = Math.Max(0, Math.Min(100, progress));
            await context.SaveChangesAsync(ct);
        }
    }

    public async Task<IEnumerable<JobDto>> GetJobsAsync(CancellationToken ct = default)
    {
        List<ProcessingJob> processingJobs = await context.Jobs.ToListAsync(ct);
        return processingJobs.Select(x => new JobDto(x) { OriginalSystemPath = x.OriginalSystemPath, FileName = x.FileName });
    }
}
