using AutoMapper;
using MassTransit;
using Users.Common;
using Users.Services.Abstracts;
using RentRide.AuthenticationApi.Models;

namespace Users.Api.Consumers;

public class AuthenticationConsumer : IConsumer<UserCreated>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    
    public AuthenticationConsumer(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var userCreated = context.Message;
        var user = _mapper.Map<UserCreated, User>(userCreated);
        await _userService.AddUserAsync(user);
    }
}

