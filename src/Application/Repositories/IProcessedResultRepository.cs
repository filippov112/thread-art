using Application.DTO;
using Application.Interfaces;
using Domain.Models;

namespace Application.Repositories;

public interface IProcessedResultRepository
{
    /// <summary>
    /// Добавляет новую запись
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>Потоки для записи файлов и пути сохранения изображений</returns>
    public Task<SavedRecord> AddProcessedResultAsync(string fileName, Stream originalStream, CancellationToken cancellationToken = default);

    public Task<IEnumerable<ImageModel>> GetAllAsync(CancellationToken cancellationToken = default);
}
