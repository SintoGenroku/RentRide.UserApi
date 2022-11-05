namespace Users.Data.Contracts;

public interface IRepository<T>
{
    Task<T> FindByIdAsync(params object[] KeyValues);

    Task<IReadOnlyCollection<T>> GetAllAsync();

    Task CreateAsync(T item);
    
    Task UpdateAsync(T item);

    Task DeleteAsync(T item);

}