namespace Application.Interfaces;

public interface IJobQueue
{
    public Task EnqueueAsync(Guid jobId, CancellationToken ct = default);
    public Task<Guid> DequeueAsync(CancellationToken ct = default);
}

