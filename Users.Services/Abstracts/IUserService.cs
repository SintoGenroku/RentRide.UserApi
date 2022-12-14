using System.Collections.ObjectModel;
using Users.Common;

namespace Users.Services.Abstracts;

public interface IUserService
{
    Task AddUserAsync(User user);
    Task<ReadOnlyCollection<User>> GetUsersAsync();

    Task DeleteUsersAsync(User user);

    Task<User> GetUserByIdAsync(Guid id);

    Task UpdateAsync(User user);

    Task<User> GetByNameAsync(string username);

    Task DeleteUserDataAsync(User user);
}