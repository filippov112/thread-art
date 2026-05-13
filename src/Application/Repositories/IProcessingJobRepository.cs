namespace Application.Repositories;

public interface IProcessingJobRepository
{
    public Task UpdateProgressAsync(Guid jobId, int progress, CancellationToken ct = default);
}
