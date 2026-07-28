using Application.QueueManager.DTO;

namespace Application.QueueManager.Repositories;

public interface IProcessingJobRepository
{
    public Task UpdateProgressAsync(Guid jobId, int progress, CancellationToken ct = default);

    public Task<IEnumerable<JobDto>> GetJobsAsync(CancellationToken ct = default);
}
