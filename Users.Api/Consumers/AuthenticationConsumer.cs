using AutoMapper;
using MassTransit;
using Users.Common;
using Users.Services.Abstracts;
using RentRide.AuthenticationApi.Models;

namespace Users.Api.Consumers;

public class AuthenticationConsumer : IConsumer<UserQueue>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    
    public AuthenticationConsumer(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<UserQueue> context)
    {
        var userQueue = context.Message;
        var user = _mapper.Map<UserQueue, User>(userQueue);
        await _userService.AddUserAsync(user);
    }
}

