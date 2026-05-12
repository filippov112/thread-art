using Domain.Models;

namespace Application.Repositories;

public interface IImageModelRepository
{
    public Task SaveImageModelAsync(ImageModel image, CancellationToken cancellationToken = default);
    public Task<IEnumerable<ImageModel>> GetAllAsync(CancellationToken cancellationToken = default);
}
