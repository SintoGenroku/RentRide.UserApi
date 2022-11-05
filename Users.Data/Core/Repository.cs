using Microsoft.EntityFrameworkCore;
using Users.Data.Contracts;

namespace Users.Data.Core;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _db;
    protected readonly DbSet<T> Data;

    public Repository(DbContext context)
    {
        _db = context;
        Data = _db.Set<T>();
    }

    public async Task<T> FindByIdAsync(params object[] keyValues)
    {
        var result = await Data.FindAsync(keyValues);

        return result;
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        var result = await Data.ToListAsync();

        return result;
    }

    public async Task CreateAsync(T item)
    {
        await Data.AddAsync(item);

        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(T item)
    {
        Data.Update(item);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(T item)
    {
        Data.Remove(item);

        await _db.SaveChangesAsync();
    }
}