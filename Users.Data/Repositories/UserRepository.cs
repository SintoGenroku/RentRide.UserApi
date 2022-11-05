using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Users.Common;
using Users.Data.Core;
using Users.Data.Repositories.Abstracts;

namespace Users.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(UsersDbContext context) : base(context)
    {
    }

    public ReadOnlyCollection<User> GetAllUsers()
    {
        var users = Data.ToList().AsReadOnly();

        return users;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        var user = await Data.FirstOrDefaultAsync(user => user.Id == id);

        return user;
    }

    public async Task<User> GetByNameAsync(string name)
    {
        var user = await Data.FirstOrDefaultAsync(user => user.Fullname == name);

        return user;
    }
}