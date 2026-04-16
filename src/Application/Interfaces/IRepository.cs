namespace Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
}
