using Application.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

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
}
