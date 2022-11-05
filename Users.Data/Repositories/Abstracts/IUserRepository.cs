using System.Collections.ObjectModel;
using Users.Common;
using Users.Data.Contracts;

namespace Users.Data.Repositories.Abstracts;

public interface IUserRepository : IRepository<User>
{
    ReadOnlyCollection<User> GetAllUsers();

    Task<User> GetByIdAsync(Guid id);

    Task<User> GetByNameAsync(string name);
}