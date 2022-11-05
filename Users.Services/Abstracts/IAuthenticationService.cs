
using RentRide.AuthenticationApi.Models.Requests;

namespace Users.Services.Abstracts;

public interface IAuthenticationService
{
    Task<string> GetTokenAsync(GetTokenRequestModel tokenRequestModel);
}