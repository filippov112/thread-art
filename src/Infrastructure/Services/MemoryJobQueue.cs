using System.Threading.Channels;
using Application.Interfaces;

namespace Infrastructure.Services;

public class MemoryJobQueue : IJobQueue
{
    private readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions { SingleReader = true });
    public Task EnqueueAsync(Guid jobId, CancellationToken ct = default) => _queue.Writer.WriteAsync(jobId, ct).AsTask();
    public async Task<Guid> DequeueAsync(CancellationToken ct = default) => await _queue.Reader.ReadAsync(ct);
}
