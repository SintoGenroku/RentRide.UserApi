using AutoMapper;
using RentRide.AuthenticationApi.Models;
using Users.Api.Consumers;
using Users.Api.Models.Requests.Users;
using Users.Api.Models.Responses.Users;
using Users.Common;

namespace Users.Api.MapperConfigs;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserCreateRequestModel, User>();
        CreateMap<UserEditRequestModel, User>();
        CreateMap<User, UserResponseModel>();
        CreateMap<UserCreated, User>();
        CreateMap<User, UserCreated>();
    }
}