using Application.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ImageModelRepository(ApplicationDbContext context) : IImageModelRepository
{
    public async Task SaveImageModelAsync(ImageModel image, CancellationToken cancellationToken = default)
    {
        await context.Images.AddAsync(image, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<ImageModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Images.ToListAsync(cancellationToken);
    }
}
