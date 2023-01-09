using RentRide.AuthenticationApi.Models.Requests;
using RentRide.Common.Exceptions;
using Users.Refit;
using Users.Services.Abstracts;

namespace Users.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationApi _authenticationApi;

    public AuthenticationService(IAuthenticationApi authenticationApi)
    {
        _authenticationApi = authenticationApi;
    }

    public async Task<string> GetTokenAsync(GetTokenRequestModel tokenRequestModel)
    {
        if (tokenRequestModel == null)
        {
            throw new BadRequestException("Request model is empty");
        }
        var tokenResponse = await _authenticationApi.GetToken(tokenRequestModel);

        return tokenResponse.access_token;
    }
}