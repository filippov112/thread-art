namespace Application.QueueManager.Repositories;

public interface IJobRepository
{
    public Task EnqueueAsync(Guid jobId, CancellationToken ct = default);
    public Task<Guid> DequeueAsync(CancellationToken ct = default);
}

