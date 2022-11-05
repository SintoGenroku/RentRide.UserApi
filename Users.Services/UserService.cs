using System.Collections.ObjectModel;
using Users.Common;
using Users.Common.Exceptions;
using Users.Data.Repositories.Abstracts;
using Users.Services.Abstracts;

namespace Users.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;


    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User doesn't exist");
        }
            
        return user;
    }

    public async Task<User> AddUserAsync(User user)
    {
        
        user.CreatedAt = DateTime.UtcNow;
        await _userRepository.CreateAsync(user);

        return user;
    }

    public async Task<ReadOnlyCollection<User>> GetUsersAsync()
    {
        var users =  _userRepository.GetAllUsers();

        return users;
    }

    public async Task DeleteUsersAsync(User user)
    { 
        await _userRepository.DeleteAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }

    public async Task<User> GetByNameAsync(string username)
    {
        var user = await _userRepository.GetByNameAsync(username);

        return user;
    }
}