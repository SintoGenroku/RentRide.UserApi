using System.Collections.ObjectModel;
using MassTransit;
using RentRide.Common.Exceptions;
using Users.Common;
using Users.Data.Repositories.Abstracts;
using Users.Services.Abstracts;

namespace Users.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IBus _bus;
    public UserService(IUserRepository userRepository, IBus bus)
    {
        _userRepository = userRepository;
        _bus = bus;
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

    public async Task AddUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        await _userRepository.CreateAsync(user);
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

    public async Task DeleteUserDataAsync(User user)
    {
        user.Fullname = null;
        user.CreatedAt = null;
        user.PhoneNumber = null;
        user.MailAddress = null;
        user.IsActive = null;
        user.IsDeleted = true;

        await _userRepository.UpdateAsync(user);

        await _bus.Publish(user.Id);
    }
}