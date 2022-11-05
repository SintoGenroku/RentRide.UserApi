using System.Net.Http.Headers;
using RentRide.AuthenticationApi.Models.Requests;
using Users.Services.Abstracts;

namespace Users.Api.Handlers;

public class AuthorizationHandler : DelegatingHandler
{
    private readonly IAuthenticationService _authenticationService;

    public AuthorizationHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestToken = new GetTokenRequestModel
        {
            client_id = "api",
            client_secret = "users-secret",
            grant_type = "client_credentials",
            scope = "ApiScope"
        };
        var token = await _authenticationService.GetTokenAsync(requestToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return await base.SendAsync(request, cancellationToken);
    }
}